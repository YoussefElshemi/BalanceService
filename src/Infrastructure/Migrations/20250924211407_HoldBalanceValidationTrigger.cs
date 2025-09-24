using Core.Enums;
using Core.Models;
using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HoldBalanceValidationTrigger : Migration
    {
        private const string ValidateHoldBalanceFunction = "validate_hold_balance";
        private const string TriggerPrefix = "tr_";

        private const int ActiveStatusId = (int)HoldStatus.Active;

        private const int PendingClosureStatusId = (int)AccountStatus.PendingClosure;
        private const int ClosedStatusId = (int)AccountStatus.Closed;

        private const string LedgerBalanceColumn = nameof(AccountEntity.LedgerBalance);
        private const string AvailableBalanceColumn = nameof(AccountEntity.AvailableBalance);
        private const string PendingCreditBalanceColumn = nameof(AccountEntity.PendingCreditBalance);
        private const string PendingDebitBalanceColumn = nameof(AccountEntity.PendingDebitBalance);
        private const string HoldBalanceColumn = nameof(AccountEntity.HoldBalance);
        private const string MinimumRequiredBalanceColumn = nameof(AccountEntity.MinimumRequiredBalance);
        private const string AccountStatusColumn = nameof(AccountEntity.AccountStatusId);

        private const string AccountIdColumn = nameof(AccountEntity.AccountId);
        private const string AmountColumn = nameof(HoldEntity.Amount);
        private const string StatusColumn = nameof(HoldEntity.HoldStatusId);
        private const string IsDeletedColumn = nameof(HoldEntity.IsDeleted);

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                create or replace function {ValidateHoldBalanceFunction}()
                returns trigger as
                $$
                declare
                    projected_available_balance numeric;
                    min_required_balance numeric;
                    acc_status int;
                    ledger_balance numeric;
                    pending_debit_balance numeric;
                    hold_balance numeric;
                    total_balance_before numeric;
                    total_balance_after numeric;
                begin
                    -- Only consider active holds that are not deleted
                    if (new.""{StatusColumn}"" = {ActiveStatusId} and new.""{IsDeletedColumn}"" = false) then
                        select ""{LedgerBalanceColumn}"",
                               ""{PendingDebitBalanceColumn}"",
                               ""{HoldBalanceColumn}"",
                               ""{MinimumRequiredBalanceColumn}"",
                               ""{AccountStatusColumn}""
                        into ledger_balance, pending_debit_balance, hold_balance, min_required_balance, acc_status
                        from ""{TableNames.Accounts}""
                        where ""{AccountIdColumn}"" = new.""{AccountIdColumn}"";

                        -- Basic projected balance check
                        projected_available_balance := ledger_balance - pending_debit_balance - (hold_balance + new.""{AmountColumn}"");

                        if (projected_available_balance < min_required_balance) then
                            raise exception '{nameof(Account.AvailableBalance)} would be reduced below the configured {nameof(Account.MinimumRequiredBalance)}. {nameof(Account.MinimumRequiredBalance)}=%, Projected {nameof(Account.AvailableBalance)}=%',
                                min_required_balance, projected_available_balance;
                        end if;

                        -- PendingClosure convergence rule
                        if (acc_status = {PendingClosureStatusId}) then
                            total_balance_before := ledger_balance - pending_debit_balance - hold_balance;
                            total_balance_after := ledger_balance - pending_debit_balance - (hold_balance + new.""{AmountColumn}"");

                            if (total_balance_after > total_balance_before) then
                                raise exception '{nameof(AccountStatus)} is {nameof(AccountStatus.PendingClosure)}, hold operation has increased balance exposure (before=%, after=%)',
                                    total_balance_before, total_balance_after;
                            end if;

                            -- Auto-close if exposure reaches zero after this hold operation
                            if (total_balance_after = 0) then
                                update ""{TableNames.Accounts}""
                                set ""{AccountStatusColumn}"" = {ClosedStatusId}
                                where ""{AccountIdColumn}"" = new.""{AccountIdColumn}"";    
                            end if;
                        end if;
                    end if;

                    return new;
                end;
                $$ language plpgsql;
            ");

            migrationBuilder.Sql($@"
                create trigger {TriggerPrefix}{ValidateHoldBalanceFunction}
                before insert or update
                on ""{TableNames.Holds}""
                for each row
                execute function {ValidateHoldBalanceFunction}();
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"drop trigger if exists {TriggerPrefix}{ValidateHoldBalanceFunction} on ""{TableNames.Holds}"";");
            migrationBuilder.Sql($@"drop function if exists {ValidateHoldBalanceFunction};");
        }
    }
}
