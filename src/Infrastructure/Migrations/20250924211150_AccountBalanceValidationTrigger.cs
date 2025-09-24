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
                    projected_available_balance numeric;
                    min_required_balance numeric;
                    acc_status int;
                    ledger_balance numeric;
                    pending_credit_balance numeric;
                    pending_debit_balance numeric;
                    hold_balance numeric;
                    total_balance_before numeric;
                    total_balance_after numeric;
                    draft_effect numeric := 0;
                begin
                    -- Always load balances
                    select ""{LedgerBalanceColumn}"",
                           ""{PendingCreditBalanceColumn}"",
                           ""{PendingDebitBalanceColumn}"",
                           ""{HoldBalanceColumn}"",
                           ""{MinimumRequiredBalanceColumn}"",
                           ""{AccountStatusColumn}""
                    into ledger_balance, pending_credit_balance, pending_debit_balance, hold_balance, min_required_balance, acc_status
                    from ""{TableNames.Accounts}""
                    where ""{AccountIdColumn}"" = new.""{AccountIdColumn}"";

                    -- Projected AvailableBalance = Ledger - PendingDebits - Hold
                    projected_available_balance := ledger_balance - pending_debit_balance - hold_balance +
                        (case
                             when new.""{DirectionColumn}"" = {CreditTransactionDirectionId} and new.""{StatusColumn}"" = {PostedStatusId} then new.""{AmountColumn}""
                             when new.""{DirectionColumn}"" = {DebitTransactionDirectionId} and new.""{StatusColumn}"" = {PostedStatusId} then -new.""{AmountColumn}""
                             else 0
                         end);

                    -- Minimum balance check only for Posted transactions
                    if (new.""{StatusColumn}"" = {PostedStatusId}) then
                        if (projected_available_balance < min_required_balance) then
                            raise exception '{nameof(Transaction)} would reduce {nameof(Account.AvailableBalance)} below {nameof(Account.MinimumRequiredBalance)}. {nameof(Account.MinimumRequiredBalance)}=%, Projected {nameof(Account.AvailableBalance)}=%',
                                min_required_balance, projected_available_balance;
                        end if;
                    end if;

                    -- PendingClosure convergence check for all transactions
                    if (acc_status = {PendingClosureStatusId}) then
                        -- net exposure before
                        total_balance_before := abs(ledger_balance - pending_debit_balance - hold_balance + pending_credit_balance);

                        -- effect depending on status
                        if (new.""{StatusColumn}"" = {PostedStatusId}) then
                            total_balance_after := abs(projected_available_balance + pending_credit_balance);
                        else
                            -- Draft affects PendingCredit / PendingDebit
                            draft_effect := case
                                                when new.""{DirectionColumn}"" = {CreditTransactionDirectionId} then new.""{AmountColumn}""
                                                when new.""{DirectionColumn}"" = {DebitTransactionDirectionId} then new.""{AmountColumn}""
                                                else 0
                                            end;

                            if (new.""{DirectionColumn}"" = {CreditTransactionDirectionId}) then
                                total_balance_after := abs(ledger_balance - pending_debit_balance - hold_balance + (pending_credit_balance + draft_effect));
                            else
                                total_balance_after := abs(ledger_balance - (pending_debit_balance + draft_effect) - hold_balance + pending_credit_balance);
                            end if;
                        end if;

                        if (total_balance_after > total_balance_before) then
                            raise exception '{nameof(AccountStatus)} is {nameof(AccountStatus.PendingClosure)}, operation has increased balance exposure (before=%, after=%)',
                                total_balance_before, total_balance_after;
                        end if;

                        -- Auto-close if all balances zero after this transaction (only Posted)
                        if (new.""{StatusColumn}"" = {PostedStatusId} and total_balance_after = 0) then
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
