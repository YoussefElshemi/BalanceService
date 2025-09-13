using Core.Enums;
using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class HoldHistoryTrigger : Migration
    {
        private const string HoldHistoryFunction = "hold_history";
        private const string TriggerPrefix = "tr_";

        private const int InsertId = (int)HistoryType.Insert;
        private const int ModifyId = (int)HistoryType.Modify;
        private const int DeleteId = (int)HistoryType.Delete;

        private static readonly string[] Columns =
        {
            nameof(HoldHistoryEntity.HoldHistoryId),
            nameof(HoldHistoryEntity.HistoryTypeId),
            nameof(HoldHistoryEntity.Timestamp),
            nameof(HoldHistoryEntity.HoldId),
            nameof(HoldHistoryEntity.AccountId),
            nameof(HoldHistoryEntity.Amount),
            nameof(HoldHistoryEntity.CurrencyCode),
            nameof(HoldHistoryEntity.IdempotencyKey),
            nameof(HoldHistoryEntity.HoldTypeId),
            nameof(HoldHistoryEntity.HoldStatusId),
            nameof(HoldHistoryEntity.HoldSourceId),
            nameof(HoldHistoryEntity.SettledTransactionId),
            nameof(HoldHistoryEntity.ExpiresAt),
            nameof(HoldHistoryEntity.Description),
            nameof(HoldHistoryEntity.Reference),
            nameof(HoldHistoryEntity.IsDeleted),
            nameof(HoldHistoryEntity.DeletedAt),
            nameof(HoldHistoryEntity.DeletedBy),
            nameof(HoldHistoryEntity.CreatedAt),
            nameof(HoldHistoryEntity.CreatedBy),
            nameof(HoldHistoryEntity.UpdatedAt),
            nameof(HoldHistoryEntity.UpdatedBy)
        };

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                create or replace function {HoldHistoryFunction}()
                returns trigger as
                $$
                begin
                    if TG_OP = 'INSERT' then
                        {BuildInsertHistorySql(TableNames.HoldHistory, Columns, InsertId, "new")}
                        return new;

                    elsif TG_OP = 'UPDATE' then
                        {BuildInsertHistorySql(TableNames.HoldHistory, Columns, ModifyId, "new")}
                        return new;

                    elsif TG_OP = 'DELETE' then
                        {BuildInsertHistorySql(TableNames.HoldHistory, Columns, DeleteId, "old")}
                        return old;
                    end if;
                end;
                $$ language plpgsql;
            ");

            migrationBuilder.Sql($@"
                create trigger {TriggerPrefix}{HoldHistoryFunction}
                after insert or update or delete
                on ""{TableNames.Holds}""
                for each row
                execute function {HoldHistoryFunction}();
            ");

            migrationBuilder.Sql(BuildBackfillSql());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"drop trigger if exists {TriggerPrefix}{HoldHistoryFunction} on ""{TableNames.Holds}"";");
            migrationBuilder.Sql($@"drop function if exists {HoldHistoryFunction};");
        }

        private static string BuildInsertHistorySql(string historyTable, string[] columns, int historyTypeId, string sourceAlias)
        {
            var cols = string.Join(",\n", columns.Select(c => $@"""{c}"""));

            var vals = string.Join(",\n", columns.Select(c =>
                c switch
                {
                    nameof(HoldHistoryEntity.HoldHistoryId) => "gen_random_uuid()",
                    nameof(HoldHistoryEntity.HistoryTypeId) => historyTypeId.ToString(),
                    nameof(HoldHistoryEntity.Timestamp) => "(now() at time zone 'utc')",
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
                    nameof(HoldHistoryEntity.HoldHistoryId) => "gen_random_uuid()",
                    nameof(HoldHistoryEntity.HistoryTypeId) => InsertId.ToString(),
                    nameof(HoldHistoryEntity.Timestamp) => "(now() at time zone 'utc')",
                    _ => $@"t.""{c}"""
                }
            ));

            return $@"
                insert into ""{TableNames.HoldHistory}"" (
                    {cols}
                )
                select
                    {vals}
                from ""{TableNames.Holds}"" t;";
        }
    }
}
