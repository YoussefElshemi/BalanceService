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

        private static readonly string[] Columns =
        {
            nameof(AccountHistoryEntity.AccountHistoryId),
            nameof(AccountHistoryEntity.HistoryTypeId),
            nameof(AccountHistoryEntity.Timestamp),
            nameof(AccountHistoryEntity.AccountId),
            nameof(AccountHistoryEntity.AccountName),
            nameof(AccountHistoryEntity.CurrencyCode),
            nameof(AccountHistoryEntity.LedgerBalance),
            nameof(AccountHistoryEntity.AvailableBalance),
            nameof(AccountHistoryEntity.PendingBalance),
            nameof(AccountHistoryEntity.HoldBalance),
            nameof(AccountHistoryEntity.MinimumRequiredBalance),
            nameof(AccountHistoryEntity.AccountTypeId),
            nameof(AccountHistoryEntity.AccountStatusId),
            nameof(AccountHistoryEntity.Metadata),
            nameof(AccountHistoryEntity.ParentAccountId),
            nameof(AccountHistoryEntity.IsDeleted),
            nameof(AccountHistoryEntity.DeletedAt),
            nameof(AccountHistoryEntity.DeletedBy),
            nameof(AccountHistoryEntity.CreatedAt),
            nameof(AccountHistoryEntity.CreatedBy),
            nameof(AccountHistoryEntity.UpdatedAt),
            nameof(AccountHistoryEntity.UpdatedBy)
        };

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                create or replace function {AccountHistoryFunction}()
                returns trigger as
                $$
                begin
                    if TG_OP = 'INSERT' then
                        {BuildInsertHistorySql(TableNames.AccountHistory, Columns, InsertId, "new")}
                        return new;

                    elsif TG_OP = 'UPDATE' then
                        {BuildInsertHistorySql(TableNames.AccountHistory, Columns, ModifyId, "new")}
                        return new;

                    elsif TG_OP = 'DELETE' then
                        {BuildInsertHistorySql(TableNames.AccountHistory, Columns, DeleteId, "old")}
                        return old;
                    end if;
                end;
                $$ language plpgsql;
            ");

            migrationBuilder.Sql($@"
                create trigger {TriggerPrefix}{AccountHistoryFunction}
                after insert or update or delete
                on ""{TableNames.Accounts}""
                for each row
                execute function {AccountHistoryFunction}();
            ");

            migrationBuilder.Sql(BuildBackfillSql());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"drop trigger if exists {TriggerPrefix}{AccountHistoryFunction} on ""{TableNames.Accounts}"";");
            migrationBuilder.Sql($@"drop function if exists {AccountHistoryFunction};");
        }

        private static string BuildInsertHistorySql(string historyTable, string[] columns, int historyTypeId, string sourceAlias)
        {
            var cols = string.Join(",\n", columns.Select(c => $@"""{c}"""));

            var vals = string.Join(",\n", columns.Select(c =>
                c switch
                {
                    nameof(AccountHistoryEntity.AccountHistoryId) => "gen_random_uuid()",
                    nameof(AccountHistoryEntity.HistoryTypeId) => historyTypeId.ToString(),
                    nameof(AccountHistoryEntity.Timestamp) => "(now() at time zone 'utc')",
                    _ => $@"{sourceAlias}.""{c}"""
                }
            ));

            return $@"
                insert into ""{historyTable}"" (
                    {cols}
                )
                values (
                    {vals}
                );";
        }

        private static string BuildBackfillSql()
        {
            var cols = string.Join(",\n", Columns.Select(c => $@"""{c}"""));

            var vals = string.Join(",\n", Columns.Select(c =>
                c switch
                {
                    nameof(AccountHistoryEntity.AccountHistoryId) => "gen_random_uuid()",
                    nameof(AccountHistoryEntity.HistoryTypeId) => InsertId.ToString(),
                    nameof(AccountHistoryEntity.Timestamp) => "(now() at time zone 'utc')",
                    _ => $@"a.""{c}"""
                }
            ));

            return $@"
                insert into ""{TableNames.AccountHistory}"" (
                    {cols}
                )
                select
                    {vals}
                from ""{TableNames.Accounts}"" a;";
        }
    }
}
