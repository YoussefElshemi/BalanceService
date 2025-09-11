using Core.Enums;
using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AccountHistoryTrigger : Migration
    {
        private const string AccountHistoryFunction = "account_history";
        private const string TriggerPrefix = "tr_";

        private const int InsertId = (int)HistoryType.Insert;
        private const int ModifyId = (int)HistoryType.Modify;
        private const int DeleteId = (int)HistoryType.Delete;

        private const string AccountHistoryIdColumn = nameof(AccountHistoryEntity.AccountHistoryId);
        private const string HistoryTypeIdColumn = nameof(AccountHistoryEntity.HistoryTypeId);
        private const string TimestampColumn = nameof(AccountHistoryEntity.Timestamp);
        private const string AccountIdColumn = nameof(AccountHistoryEntity.AccountId);
        private const string AccountNameColumn = nameof(AccountHistoryEntity.AccountName);
        private const string CurrencyCodeColumn = nameof(AccountHistoryEntity.CurrencyCode);
        private const string LedgerBalanceColumn = nameof(AccountHistoryEntity.LedgerBalance);
        private const string AvailableBalanceColumn = nameof(AccountHistoryEntity.AvailableBalance);
        private const string PendingBalanceColumn = nameof(AccountHistoryEntity.PendingBalance);
        private const string HoldBalanceColumn = nameof(AccountHistoryEntity.HoldBalance);
        private const string MinimumRequiredBalanceColumn = nameof(AccountHistoryEntity.MinimumRequiredBalance);
        private const string AccountTypeIdColumn = nameof(AccountHistoryEntity.AccountTypeId);
        private const string AccountStatusIdColumn = nameof(AccountHistoryEntity.AccountStatusId);
        private const string MetadataColumn = nameof(AccountHistoryEntity.Metadata);
        private const string ParentAccountIdColumn = nameof(AccountHistoryEntity.ParentAccountId);
        private const string IsDeletedColumn = nameof(AccountHistoryEntity.IsDeleted);
        private const string DeletedAtColumn = nameof(AccountHistoryEntity.DeletedAt);
        private const string DeletedByColumn = nameof(AccountHistoryEntity.DeletedBy);
        private const string CreatedAtColumn = nameof(AccountHistoryEntity.CreatedAt);
        private const string CreatedByColumn = nameof(AccountHistoryEntity.CreatedBy);
        private const string UpdatedAtColumn = nameof(AccountHistoryEntity.UpdatedAt);
        private const string UpdatedByColumn = nameof(AccountHistoryEntity.UpdatedBy);

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Trigger function
            migrationBuilder.Sql($@"
                create or replace function {AccountHistoryFunction}() 
                returns trigger as
                $$
                begin
                    if TG_OP = 'INSERT' then
                        insert into ""{TableNames.AccountHistory}"" (
                            ""{AccountHistoryIdColumn}"",
                            ""{HistoryTypeIdColumn}"",
                            ""{TimestampColumn}"",
                            ""{AccountIdColumn}"",
                            ""{AccountNameColumn}"",
                            ""{CurrencyCodeColumn}"",
                            ""{LedgerBalanceColumn}"",
                            ""{AvailableBalanceColumn}"",
                            ""{PendingBalanceColumn}"",
                            ""{HoldBalanceColumn}"",
                            ""{MinimumRequiredBalanceColumn}"",
                            ""{AccountTypeIdColumn}"",
                            ""{AccountStatusIdColumn}"",
                            ""{MetadataColumn}"",
                            ""{ParentAccountIdColumn}"",
                            ""{IsDeletedColumn}"",
                            ""{DeletedAtColumn}"",
                            ""{DeletedByColumn}"",
                            ""{CreatedAtColumn}"",
                            ""{CreatedByColumn}"",
                            ""{UpdatedAtColumn}"",
                            ""{UpdatedByColumn}""
                        )
                        values (
                            gen_random_uuid(),
                            {InsertId},
                            (now() at time zone 'utc'),
                            new.""{AccountIdColumn}"",
                            new.""{AccountNameColumn}"",
                            new.""{CurrencyCodeColumn}"",
                            new.""{LedgerBalanceColumn}"",
                            new.""{AvailableBalanceColumn}"",
                            new.""{PendingBalanceColumn}"",
                            new.""{HoldBalanceColumn}"",
                            new.""{MinimumRequiredBalanceColumn}"",
                            new.""{AccountTypeIdColumn}"",
                            new.""{AccountStatusIdColumn}"",
                            new.""{MetadataColumn}"",
                            new.""{ParentAccountIdColumn}"",
                            new.""{IsDeletedColumn}"",
                            new.""{DeletedAtColumn}"",
                            new.""{DeletedByColumn}"",
                            new.""{CreatedAtColumn}"",
                            new.""{CreatedByColumn}"",
                            new.""{UpdatedAtColumn}"",
                            new.""{UpdatedByColumn}""
                        );
                        return new;

                    elsif TG_OP = 'UPDATE' then
                        insert into ""{TableNames.AccountHistory}"" (
                            ""{AccountHistoryIdColumn}"",
                            ""{HistoryTypeIdColumn}"",
                            ""{TimestampColumn}"",
                            ""{AccountIdColumn}"",
                            ""{AccountNameColumn}"",
                            ""{CurrencyCodeColumn}"",
                            ""{LedgerBalanceColumn}"",
                            ""{AvailableBalanceColumn}"",
                            ""{PendingBalanceColumn}"",
                            ""{HoldBalanceColumn}"",
                            ""{MinimumRequiredBalanceColumn}"",
                            ""{AccountTypeIdColumn}"",
                            ""{AccountStatusIdColumn}"",
                            ""{MetadataColumn}"",
                            ""{ParentAccountIdColumn}"",
                            ""{IsDeletedColumn}"",
                            ""{DeletedAtColumn}"",
                            ""{DeletedByColumn}"",
                            ""{CreatedAtColumn}"",
                            ""{CreatedByColumn}"",
                            ""{UpdatedAtColumn}"",
                            ""{UpdatedByColumn}""
                        )
                        values (
                            gen_random_uuid(),
                            {ModifyId},
                            (now() at time zone 'utc'),
                            new.""{AccountIdColumn}"",
                            new.""{AccountNameColumn}"",
                            new.""{CurrencyCodeColumn}"",
                            new.""{LedgerBalanceColumn}"",
                            new.""{AvailableBalanceColumn}"",
                            new.""{PendingBalanceColumn}"",
                            new.""{HoldBalanceColumn}"",
                            new.""{MinimumRequiredBalanceColumn}"",
                            new.""{AccountTypeIdColumn}"",
                            new.""{AccountStatusIdColumn}"",
                            new.""{MetadataColumn}"",
                            new.""{ParentAccountIdColumn}"",
                            new.""{IsDeletedColumn}"",
                            new.""{DeletedAtColumn}"",
                            new.""{DeletedByColumn}"",
                            new.""{CreatedAtColumn}"",
                            new.""{CreatedByColumn}"",
                            new.""{UpdatedAtColumn}"",
                            new.""{UpdatedByColumn}""
                        );
                        return new;

                    elsif TG_OP = 'DELETE' then
                        insert into ""{TableNames.AccountHistory}"" (
                            ""{AccountHistoryIdColumn}"",
                            ""{HistoryTypeIdColumn}"",
                            ""{TimestampColumn}"",
                            ""{AccountIdColumn}"",
                            ""{AccountNameColumn}"",
                            ""{CurrencyCodeColumn}"",
                            ""{LedgerBalanceColumn}"",
                            ""{AvailableBalanceColumn}"",
                            ""{PendingBalanceColumn}"",
                            ""{HoldBalanceColumn}"",
                            ""{MinimumRequiredBalanceColumn}"",
                            ""{AccountTypeIdColumn}"",
                            ""{AccountStatusIdColumn}"",
                            ""{MetadataColumn}"",
                            ""{ParentAccountIdColumn}"",
                            ""{IsDeletedColumn}"",
                            ""{DeletedAtColumn}"",
                            ""{DeletedByColumn}"",
                            ""{CreatedAtColumn}"",
                            ""{CreatedByColumn}"",
                            ""{UpdatedAtColumn}"",
                            ""{UpdatedByColumn}""
                        )
                        values (
                            gen_random_uuid(),
                            {DeleteId},
                            (now() at time zone 'utc'),
                            old.""{AccountIdColumn}"",
                            old.""{AccountNameColumn}"",
                            old.""{CurrencyCodeColumn}"",
                            old.""{LedgerBalanceColumn}"",
                            old.""{AvailableBalanceColumn}"",
                            old.""{PendingBalanceColumn}"",
                            old.""{HoldBalanceColumn}"",
                            old.""{MinimumRequiredBalanceColumn}"",
                            old.""{AccountTypeIdColumn}"",
                            old.""{AccountStatusIdColumn}"",
                            old.""{MetadataColumn}"",
                            old.""{ParentAccountIdColumn}"",
                            old.""{IsDeletedColumn}"",
                            old.""{DeletedAtColumn}"",
                            old.""{DeletedByColumn}"",
                            old.""{CreatedAtColumn}"",
                            old.""{CreatedByColumn}"",
                            old.""{UpdatedAtColumn}"",
                            old.""{UpdatedByColumn}""
                        );
                        return old;
                    end if;
                end;
                $$ language plpgsql;
            ");

            // Trigger
            migrationBuilder.Sql($@"
                create trigger {TriggerPrefix}{AccountHistoryFunction}
                after insert or update or delete
                on ""{TableNames.Accounts}""
                for each row
                execute function {AccountHistoryFunction}();
            ");

            // Backfill existing Accounts
            migrationBuilder.Sql($@"
                insert into ""{TableNames.AccountHistory}"" (
                    ""{AccountHistoryIdColumn}"",
                    ""{HistoryTypeIdColumn}"",
                    ""{TimestampColumn}"",
                    ""{AccountIdColumn}"",
                    ""{AccountNameColumn}"",
                    ""{CurrencyCodeColumn}"",
                    ""{LedgerBalanceColumn}"",
                    ""{AvailableBalanceColumn}"",
                    ""{PendingBalanceColumn}"",
                    ""{HoldBalanceColumn}"",
                    ""{MinimumRequiredBalanceColumn}"",
                    ""{AccountTypeIdColumn}"",
                    ""{AccountStatusIdColumn}"",
                    ""{MetadataColumn}"",
                    ""{ParentAccountIdColumn}"",
                    ""{IsDeletedColumn}"",
                    ""{DeletedAtColumn}"",
                    ""{DeletedByColumn}"",
                    ""{CreatedAtColumn}"",
                    ""{CreatedByColumn}"",
                    ""{UpdatedAtColumn}"",
                    ""{UpdatedByColumn}""
                )
                select
                    gen_random_uuid(),
                    {InsertId},
                    (now() at time zone 'utc'),
                    a.""{AccountIdColumn}"",
                    a.""{AccountNameColumn}"",
                    a.""{CurrencyCodeColumn}"",
                    a.""{LedgerBalanceColumn}"",
                    a.""{AvailableBalanceColumn}"",
                    a.""{PendingBalanceColumn}"",
                    a.""{HoldBalanceColumn}"",
                    a.""{MinimumRequiredBalanceColumn}"",
                    a.""{AccountTypeIdColumn}"",
                    a.""{AccountStatusIdColumn}"",
                    a.""{MetadataColumn}"",
                    a.""{ParentAccountIdColumn}"",
                    a.""{IsDeletedColumn}"",
                    a.""{DeletedAtColumn}"",
                    a.""{DeletedByColumn}"",
                    a.""{CreatedAtColumn}"",
                    a.""{CreatedByColumn}"",
                    a.""{UpdatedAtColumn}"",
                    a.""{UpdatedByColumn}""
                from ""{TableNames.Accounts}"" a;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"drop trigger if exists {TriggerPrefix}{AccountHistoryFunction} on ""{TableNames.Accounts}"";");
            migrationBuilder.Sql($@"drop function if exists {AccountHistoryFunction};");
        }
    }
}
