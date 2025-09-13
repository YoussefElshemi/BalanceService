using Core.Models;
using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AccountValidationTrigger : Migration
    {
        private const string ValidateMinBalanceUpdateFunction = "validate_min_balance_update";
        private const string TriggerPrefix = "tr_";

        private const string AvailableBalanceColumn = nameof(AccountEntity.AvailableBalance);
        private const string MinimumRequiredBalanceColumn = nameof(AccountEntity.MinimumRequiredBalance);

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.Sql($@"
                create or replace function {ValidateMinBalanceUpdateFunction}()
                returns trigger as
                $$
                begin
                    if (new.""{MinimumRequiredBalanceColumn}"" > old.""{MinimumRequiredBalanceColumn}"") then
                        if (new.""{AvailableBalanceColumn}"" < new.""{MinimumRequiredBalanceColumn}"") then
                            raise exception 'Cannot increase {nameof(Account.MinimumRequiredBalance)} above {nameof(Account.AvailableBalance)}: %',
                                new.""{AvailableBalanceColumn}"";
                        end if;
                    end if;
                    return new;
                end;
                $$ language plpgsql;
            ");

            migrationBuilder.Sql($@"
                create trigger {TriggerPrefix}{ValidateMinBalanceUpdateFunction}
                before update of ""{MinimumRequiredBalanceColumn}""
                on ""{TableNames.Accounts}""
                for each row
                execute function {ValidateMinBalanceUpdateFunction}();
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"drop trigger if exists {TriggerPrefix}{ValidateMinBalanceUpdateFunction} on ""{TableNames.Accounts}"";");
            migrationBuilder.Sql($@"drop function if exists {ValidateMinBalanceUpdateFunction};");
        }
    }
}
