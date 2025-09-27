using Core.Enums;
using Core.Models;
using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AccountBalanceValidationTrigger : Migration
    {
        private const string ValidateAccountBalanceFunction = "validate_account_balance";
        private const string TriggerPrefix = "tr_";

        private const int CreditTransactionDirectionId = (int)TransactionDirection.Credit;
        private const int DebitTransactionDirectionId = (int)TransactionDirection.Debit;

        private const int DraftStatusId = (int)TransactionStatus.Draft;
        private const int PostedStatusId = (int)TransactionStatus.Posted;

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
        private const string AmountColumn = nameof(TransactionEntity.Amount);
        private const string DirectionColumn = nameof(TransactionEntity.TransactionDirectionId);
        private const string StatusColumn = nameof(TransactionEntity.TransactionStatusId);

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                create or replace function {ValidateAccountBalanceFunction}()
                returns trigger as
                $$
                declare
                    ledger_balance numeric;
                    available_balance numeric;
                    pending_credit_balance numeric;
                    pending_debit_balance numeric;
                    hold_balance numeric;
                    min_required_balance numeric;
                    acc_status int;

                    projected_ledger_balance numeric;
                    projected_available_balance numeric;
                    projected_pending_credit_balance numeric;
                    projected_pending_debit_balance numeric;

                    total_before numeric;
                    total_after numeric;
                begin
                    -- Always load current balances
                    select ""{LedgerBalanceColumn}"",
                           ""{AvailableBalanceColumn}"",
                           ""{PendingCreditBalanceColumn}"",
                           ""{PendingDebitBalanceColumn}"",
                           ""{HoldBalanceColumn}"",
                           ""{MinimumRequiredBalanceColumn}"",
                           ""{AccountStatusColumn}""
                    into ledger_balance, available_balance, pending_credit_balance, pending_debit_balance, hold_balance, min_required_balance, acc_status
                    from ""{TableNames.Accounts}""
                    where ""{AccountIdColumn}"" = new.""{AccountIdColumn}"";

                    -- Start from current balances
                    projected_ledger_balance := ledger_balance;
                    projected_available_balance := available_balance;
                    projected_pending_credit_balance := pending_credit_balance;
                    projected_pending_debit_balance := pending_debit_balance;

                    -- Apply same effect as update_account_balance (validation only)
                    if (new.""{StatusColumn}"" = {DraftStatusId}) then
                        if (new.""{DirectionColumn}"" = {CreditTransactionDirectionId}) then
                            projected_pending_credit_balance := projected_pending_credit_balance + new.""{AmountColumn}"";
                        elsif (new.""{DirectionColumn}"" = {DebitTransactionDirectionId}) then
                            projected_pending_debit_balance := projected_pending_debit_balance + new.""{AmountColumn}"";
                            projected_available_balance := projected_available_balance - new.""{AmountColumn}"";
                        end if;
                    elsif (new.""{StatusColumn}"" = {PostedStatusId}) then
                        if (new.""{DirectionColumn}"" = {CreditTransactionDirectionId}) then
                            projected_ledger_balance := projected_ledger_balance + new.""{AmountColumn}"";
                            projected_available_balance := projected_available_balance + new.""{AmountColumn}"";
                        elsif (new.""{DirectionColumn}"" = {DebitTransactionDirectionId}) then
                            projected_ledger_balance := projected_ledger_balance - new.""{AmountColumn}"";
                            projected_available_balance := projected_available_balance - new.""{AmountColumn}"";
                        end if;
                    end if;

                    -- Check minimum balance rule
                    if (projected_available_balance < min_required_balance) then
                        raise exception '{nameof(Transaction)} would reduce {nameof(Account.AvailableBalance)} below {nameof(Account.MinimumRequiredBalance)}. {nameof(Account.MinimumRequiredBalance)}=%, Projected {nameof(Account.AvailableBalance)}=%',
                            min_required_balance, projected_available_balance;
                    end if;

                    -- PendingClosure rules
                    if (acc_status = {PendingClosureStatusId}) then
                        total_before := abs(ledger_balance - pending_debit_balance - hold_balance + pending_credit_balance);
                        total_after := abs(projected_ledger_balance - projected_pending_debit_balance - hold_balance + projected_pending_credit_balance);

                        if (total_after > total_before) then
                            raise exception '{nameof(AccountStatus)} is {nameof(AccountStatus.PendingClosure)}, operation has increased balance exposure (before=%, after=%)',
                                total_before, total_after;
                        end if;

                        -- Auto-close if all balances converge to zero
                        if (new.""{StatusColumn}"" = {PostedStatusId}
                            and projected_ledger_balance = 0
                            and projected_available_balance = 0
                            and projected_pending_credit_balance = 0
                            and projected_pending_debit_balance = 0
                            and hold_balance = 0) then
                            update ""{TableNames.Accounts}""
                            set ""{AccountStatusColumn}"" = {ClosedStatusId}
                            where ""{AccountIdColumn}"" = new.""{AccountIdColumn}"";
                        end if;
                    end if;

                    return new;
                end;
                $$ language plpgsql;
            ");

            migrationBuilder.Sql($@"
                drop trigger if exists {TriggerPrefix}{ValidateAccountBalanceFunction} on ""{TableNames.Transactions}"";
                create trigger {TriggerPrefix}{ValidateAccountBalanceFunction}
                before insert or update
                on ""{TableNames.Transactions}""
                for each row
                execute function {ValidateAccountBalanceFunction}();
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"drop trigger if exists {TriggerPrefix}{ValidateAccountBalanceFunction} on ""{TableNames.Transactions}"";");
            migrationBuilder.Sql($@"drop function if exists {ValidateAccountBalanceFunction};");
        }
    }
}
