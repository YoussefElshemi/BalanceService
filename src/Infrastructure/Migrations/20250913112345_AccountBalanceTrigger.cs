using Core.Enums;
using Core.Models;
using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AccountBalanceTrigger : Migration
    {
        private const string UpdateAccountBalanceFunction = "update_account_balance";
        private const string TriggerPrefix = "tr_";

        private const int TransactionDirectionCreditId = (int)TransactionDirection.Credit;
        private const int TransactionDirectionDebitId = (int)TransactionDirection.Debit;
        private const int TransactionStatusDraftId = (int)TransactionStatus.Draft;
        private const int TransactionStatusPostedId = (int)TransactionStatus.Posted;

        private const string AccountIdColumn = nameof(AccountEntity.AccountId);
        private const string LedgerBalanceColumn = nameof(AccountEntity.LedgerBalance);
        private const string AvailableBalanceColumn = nameof(AccountEntity.AvailableBalance);
        private const string PendingBalanceColumn = nameof(AccountEntity.PendingBalance);

        private const string AmountColumn = nameof(TransactionEntity.Amount);
        private const string TransactionDirectionIdColumn = nameof(TransactionEntity.TransactionDirectionId);
        private const string TransactionStatusIdColumn = nameof(TransactionStatusEntity.TransactionStatusId);

        private const string UpdatedByColumn = nameof(BaseEntity.UpdatedBy);
        private const string UpdatedAtColumn = nameof(BaseEntity.UpdatedAt);
        private const string IsDeletedColumn = nameof(DeletableBaseEntity.IsDeleted);
        private const string DeletedByColumn = nameof(DeletableBaseEntity.DeletedBy);
        private const string DeletedAtColumn = nameof(DeletableBaseEntity.DeletedAt);

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                create or replace function {UpdateAccountBalanceFunction}()
                returns trigger as
                $$
                declare
                    old_amount numeric := (
                        case
                            when TG_OP <> 'INSERT' and old.""{TransactionDirectionIdColumn}"" = {TransactionDirectionCreditId} then old.""{AmountColumn}""
                            when TG_OP <> 'INSERT' and old.""{TransactionDirectionIdColumn}"" = {TransactionDirectionDebitId} then -old.""{AmountColumn}""
                            else 0
                        end
                    );
                    new_amount numeric := (
                        case
                            when TG_OP <> 'DELETE' and new.""{TransactionDirectionIdColumn}"" = {TransactionDirectionCreditId} then new.""{AmountColumn}""
                            when TG_OP <> 'DELETE' and new.""{TransactionDirectionIdColumn}"" = {TransactionDirectionDebitId} then -new.""{AmountColumn}""
                            else 0
                        end
                    );
                begin
                    -- INSERT
                    if (TG_OP = 'INSERT') then
                        if (new.""{IsDeletedColumn}"" = false) then
                            if (new.""{TransactionStatusIdColumn}"" = {TransactionStatusDraftId}) then
                                update ""{TableNames.Accounts}""
                                set ""{PendingBalanceColumn}"" = ""{PendingBalanceColumn}"" + new_amount,
                                    ""{UpdatedByColumn}"" = new.""{UpdatedByColumn}"",
                                    ""{UpdatedAtColumn}"" = new.""{UpdatedAtColumn}""
                                where ""{AccountIdColumn}"" = new.""{AccountIdColumn}"";
                            elsif (new.""{TransactionStatusIdColumn}"" = {TransactionStatusPostedId}) then
                                update ""{TableNames.Accounts}""
                                set ""{LedgerBalanceColumn}"" = ""{LedgerBalanceColumn}"" + new_amount,
                                    ""{AvailableBalanceColumn}"" = ""{AvailableBalanceColumn}"" + new_amount,
                                    ""{UpdatedByColumn}"" = new.""{UpdatedByColumn}"",
                                    ""{UpdatedAtColumn}"" = new.""{UpdatedAtColumn}""
                                where ""{AccountIdColumn}"" = new.""{AccountIdColumn}"";
                            end if;
                        end if;
                        return new;
                    end if;

                    -- UPDATE
                    if (TG_OP = 'UPDATE') then
                        -- AccountId changed
                        if (old.""{AccountIdColumn}"" <> new.""{AccountIdColumn}"") then
                            -- remove from old account
                            if (old.""{TransactionStatusIdColumn}"" = {TransactionStatusDraftId} and old.""{IsDeletedColumn}"" = false) then
                                update ""{TableNames.Accounts}""
                                set ""{PendingBalanceColumn}"" = ""{PendingBalanceColumn}"" - old_amount,
                                    ""{UpdatedByColumn}"" = new.""{UpdatedByColumn}"",
                                    ""{UpdatedAtColumn}"" = new.""{UpdatedAtColumn}""
                                where ""{AccountIdColumn}"" = old.""{AccountIdColumn}"";
                            elsif (old.""{TransactionStatusIdColumn}"" = {TransactionStatusPostedId} and old.""{IsDeletedColumn}"" = false) then
                                update ""{TableNames.Accounts}""
                                set ""{LedgerBalanceColumn}"" = ""{LedgerBalanceColumn}"" - old_amount,
                                    ""{AvailableBalanceColumn}"" = ""{AvailableBalanceColumn}"" - old_amount,
                                    ""{UpdatedByColumn}"" = new.""{UpdatedByColumn}"",
                                    ""{UpdatedAtColumn}"" = new.""{UpdatedAtColumn}""
                                where ""{AccountIdColumn}"" = old.""{AccountIdColumn}"";
                            end if;

                            -- add to new account
                            if (new.""{TransactionStatusIdColumn}"" = {TransactionStatusDraftId} and new.""{IsDeletedColumn}"" = false) then
                                update ""{TableNames.Accounts}""
                                set ""{PendingBalanceColumn}"" = ""{PendingBalanceColumn}"" + new_amount,
                                    ""{UpdatedByColumn}"" = new.""{UpdatedByColumn}"",
                                    ""{UpdatedAtColumn}"" = new.""{UpdatedAtColumn}""
                                where ""{AccountIdColumn}"" = new.""{AccountIdColumn}"";
                            elsif (new.""{TransactionStatusIdColumn}"" = {TransactionStatusPostedId} and new.""{IsDeletedColumn}"" = false) then
                                update ""{TableNames.Accounts}""
                                set ""{LedgerBalanceColumn}"" = ""{LedgerBalanceColumn}"" + new_amount,
                                    ""{AvailableBalanceColumn}"" = ""{AvailableBalanceColumn}"" + new_amount,
                                    ""{UpdatedByColumn}"" = new.""{UpdatedByColumn}"",
                                    ""{UpdatedAtColumn}"" = new.""{UpdatedAtColumn}""
                                where ""{AccountIdColumn}"" = new.""{AccountIdColumn}"";
                            end if;

                            return new;
                        end if;

                        -- Draft -> Posted
                        if (old.""{TransactionStatusIdColumn}"" = {TransactionStatusDraftId}
                            and new.""{TransactionStatusIdColumn}"" = {TransactionStatusPostedId}
                            and new.""{IsDeletedColumn}"" = false) then
                            update ""{TableNames.Accounts}""
                            set ""{PendingBalanceColumn}"" = ""{PendingBalanceColumn}"" - old_amount,
                                ""{LedgerBalanceColumn}"" = ""{LedgerBalanceColumn}"" + new_amount,
                                ""{AvailableBalanceColumn}"" = ""{AvailableBalanceColumn}"" + new_amount,
                                    ""{UpdatedByColumn}"" = new.""{UpdatedByColumn}"",
                                    ""{UpdatedAtColumn}"" = new.""{UpdatedAtColumn}""
                            where ""{AccountIdColumn}"" = new.""{AccountIdColumn}"";
                        end if;

                        -- Amount / Direction change
                        if (old.""{TransactionStatusIdColumn}"" = new.""{TransactionStatusIdColumn}"" 
                            and old.""{AccountIdColumn}"" = new.""{AccountIdColumn}"" 
                            and (old.""{AmountColumn}"" <> new.""{AmountColumn}"" 
                                 or old.""{TransactionDirectionIdColumn}"" <> new.""{TransactionDirectionIdColumn}"")
                            and new.""{IsDeletedColumn}"" = false) then
                            if (new.""{TransactionStatusIdColumn}"" = {TransactionStatusDraftId}) then
                                update ""{TableNames.Accounts}""
                                set ""{PendingBalanceColumn}"" = ""{PendingBalanceColumn}"" - old_amount + new_amount,
                                    ""{UpdatedByColumn}"" = new.""{UpdatedByColumn}"",
                                    ""{UpdatedAtColumn}"" = new.""{UpdatedAtColumn}""
                                where ""{AccountIdColumn}"" = new.""{AccountIdColumn}"";
                            elsif (new.""{TransactionStatusIdColumn}"" = {TransactionStatusPostedId}) then
                                update ""{TableNames.Accounts}""
                                set ""{LedgerBalanceColumn}"" = ""{LedgerBalanceColumn}"" - old_amount + new_amount,
                                    ""{AvailableBalanceColumn}"" = ""{AvailableBalanceColumn}"" - old_amount + new_amount,
                                    ""{UpdatedByColumn}"" = new.""{UpdatedByColumn}"",
                                    ""{UpdatedAtColumn}"" = new.""{UpdatedAtColumn}""
                                where ""{AccountIdColumn}"" = new.""{AccountIdColumn}"";
                            end if;
                        end if;

                        -- Soft delete
                        if (old.""{IsDeletedColumn}"" = false and new.""{IsDeletedColumn}"" = true) then
                            if (old.""{TransactionStatusIdColumn}"" = {TransactionStatusDraftId}) then
                                update ""{TableNames.Accounts}""
                                set ""{PendingBalanceColumn}"" = ""{PendingBalanceColumn}"" - old_amount,
                                    ""{UpdatedByColumn}"" = new.""{DeletedByColumn}"",
                                    ""{UpdatedAtColumn}"" = new.""{DeletedAtColumn}""
                                where ""{AccountIdColumn}"" = old.""{AccountIdColumn}"";
                            elsif (old.""{TransactionStatusIdColumn}"" = {TransactionStatusPostedId}) then
                                update ""{TableNames.Accounts}""
                                set ""{LedgerBalanceColumn}"" = ""{LedgerBalanceColumn}"" - old_amount,
                                    ""{AvailableBalanceColumn}"" = ""{AvailableBalanceColumn}"" - old_amount,
                                    ""{UpdatedByColumn}"" = new.""{DeletedByColumn}"",
                                    ""{UpdatedAtColumn}"" = new.""{DeletedAtColumn}""
                                where ""{AccountIdColumn}"" = old.""{AccountIdColumn}"";
                            end if;
                        end if;

                        -- Restore from soft delete
                        if (old.""{IsDeletedColumn}"" = true and new.""{IsDeletedColumn}"" = false) then
                            if (new.""{TransactionStatusIdColumn}"" = {TransactionStatusDraftId}) then
                                update ""{TableNames.Accounts}""
                                set ""{PendingBalanceColumn}"" = ""{PendingBalanceColumn}"" + new_amount,
                                    ""{UpdatedAtColumn}"" = now() at time zone 'utc'
                                where ""{AccountIdColumn}"" = new.""{AccountIdColumn}"";
                            elsif (new.""{TransactionStatusIdColumn}"" = {TransactionStatusPostedId}) then
                                update ""{TableNames.Accounts}""
                                set ""{LedgerBalanceColumn}"" = ""{LedgerBalanceColumn}"" + new_amount,
                                    ""{AvailableBalanceColumn}"" = ""{AvailableBalanceColumn}"" + new_amount,
                                    ""{UpdatedAtColumn}"" = now() at time zone 'utc'
                                where ""{AccountIdColumn}"" = new.""{AccountIdColumn}"";
                            end if;
                        end if;

                        return new;
                    end if;

                    -- DELETE
                    if (TG_OP = 'DELETE') then
                        if (old.""{IsDeletedColumn}"" = false) then
                            if (old.""{TransactionStatusIdColumn}"" = {TransactionStatusDraftId}) then
                                update ""{TableNames.Accounts}""
                                set ""{PendingBalanceColumn}"" = ""{PendingBalanceColumn}"" - old_amount,
                                    ""{UpdatedByColumn}"" = old.""{UpdatedByColumn}"",
                                    ""{UpdatedAtColumn}"" = old.""{UpdatedAtColumn}""
                                where ""{AccountIdColumn}"" = old.""{AccountIdColumn}"";
                            elsif (old.""{TransactionStatusIdColumn}"" = {TransactionStatusPostedId}) then
                                update ""{TableNames.Accounts}""
                                set ""{LedgerBalanceColumn}"" = ""{LedgerBalanceColumn}"" - old_amount,
                                    ""{AvailableBalanceColumn}"" = ""{AvailableBalanceColumn}"" - old_amount,
                                    ""{UpdatedAtColumn}"" = now() at time zone 'utc'
                                where ""{AccountIdColumn}"" = old.""{AccountIdColumn}"";
                            end if;
                        end if;
                        return old;
                    end if;

                    return null;
                end;
                $$ language plpgsql;
            ");

            migrationBuilder.Sql($@"
                drop trigger if exists {TriggerPrefix}{UpdateAccountBalanceFunction} on ""{TableNames.Transactions}"";
                create trigger {TriggerPrefix}{UpdateAccountBalanceFunction}
                after insert or update or delete
                on ""{TableNames.Transactions}""
                for each row
                execute function {UpdateAccountBalanceFunction}();
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"drop trigger if exists {TriggerPrefix}{UpdateAccountBalanceFunction} on ""{TableNames.Transactions}"";");
            migrationBuilder.Sql($@"drop function if exists {UpdateAccountBalanceFunction};");
        }
    }
}
