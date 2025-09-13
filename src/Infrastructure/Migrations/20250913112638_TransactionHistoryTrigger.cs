using Core.Enums;
using Infrastructure.Constants;
using Infrastructure.Entities;
using Infrastructure.Entities.History;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class TransactionHistoryTrigger : Migration
    {
        private const string TransactionHistoryFunction = "transaction_history";
        private const string TriggerPrefix = "tr_";

        private const int InsertId = (int)HistoryType.Insert;
        private const int ModifyId = (int)HistoryType.Modify;
        private const int DeleteId = (int)HistoryType.Delete;

        private static readonly string[] Columns =
        {
            nameof(TransactionHistoryEntity.TransactionHistoryId),
            nameof(TransactionHistoryEntity.HistoryTypeId),
            nameof(TransactionHistoryEntity.Timestamp),
            nameof(TransactionHistoryEntity.TransactionId),
            nameof(TransactionHistoryEntity.AccountId),
            nameof(TransactionHistoryEntity.Amount),
            nameof(TransactionHistoryEntity.CurrencyCode),
            nameof(TransactionHistoryEntity.TransactionDirectionId),
            nameof(TransactionHistoryEntity.PostedAt),
            nameof(TransactionHistoryEntity.IdempotencyKey),
            nameof(TransactionHistoryEntity.TransactionTypeId),
            nameof(TransactionHistoryEntity.TransactionStatusId),
            nameof(TransactionHistoryEntity.TransactionSourceId),
            nameof(TransactionHistoryEntity.Description),
            nameof(TransactionHistoryEntity.Reference),
            nameof(TransactionHistoryEntity.IsDeleted),
            nameof(TransactionHistoryEntity.DeletedAt),
            nameof(TransactionHistoryEntity.DeletedBy),
            nameof(TransactionHistoryEntity.CreatedAt),
            nameof(TransactionHistoryEntity.CreatedBy),
            nameof(TransactionHistoryEntity.UpdatedAt),
            nameof(TransactionHistoryEntity.UpdatedBy)
        };

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                create or replace function {TransactionHistoryFunction}()
                returns trigger as
                $$
                begin
                    if TG_OP = 'INSERT' then
                        {BuildInsertHistorySql(TableNames.TransactionHistory, Columns, InsertId, "new")}
                        return new;

                    elsif TG_OP = 'UPDATE' then
                        {BuildInsertHistorySql(TableNames.TransactionHistory, Columns, ModifyId, "new")}
                        return new;

                    elsif TG_OP = 'DELETE' then
                        {BuildInsertHistorySql(TableNames.TransactionHistory, Columns, DeleteId, "old")}
                        return old;
                    end if;
                end;
                $$ language plpgsql;
            ");

            migrationBuilder.Sql($@"
                create trigger {TriggerPrefix}{TransactionHistoryFunction}
                after insert or update or delete
                on ""{TableNames.Transactions}""
                for each row
                execute function {TransactionHistoryFunction}();
            ");

            migrationBuilder.Sql(BuildBackfillSql());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"drop trigger if exists {TriggerPrefix}{TransactionHistoryFunction} on ""{TableNames.Transactions}"";");
            migrationBuilder.Sql($@"drop function if exists {TransactionHistoryFunction};");
        }

        private static string BuildInsertHistorySql(string historyTable, string[] columns, int historyTypeId, string sourceAlias)
        {
            var cols = string.Join(",\n", columns.Select(c => $@"""{c}"""));

            var vals = string.Join(",\n", columns.Select(c =>
                c switch
                {
                    nameof(TransactionHistoryEntity.TransactionHistoryId) => "gen_random_uuid()",
                    nameof(TransactionHistoryEntity.HistoryTypeId) => historyTypeId.ToString(),
                    nameof(TransactionHistoryEntity.Timestamp) => "(now() at time zone 'utc')",
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
                    nameof(TransactionHistoryEntity.TransactionHistoryId) => "gen_random_uuid()",
                    nameof(TransactionHistoryEntity.HistoryTypeId) => InsertId.ToString(),
                    nameof(TransactionHistoryEntity.Timestamp) => "(now() at time zone 'utc')",
                    _ => $@"t.""{c}"""
                }
            ));

            return $@"
                insert into ""{TableNames.TransactionHistory}"" (
                    {cols}
                )
                select
                    {vals}
                from ""{TableNames.Transactions}"" t;";
        }
    }
}
