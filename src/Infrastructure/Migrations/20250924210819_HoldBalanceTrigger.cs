using Core.Enums;
using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

public partial class HoldBalanceTrigger : Migration
{
    private const string UpdateHoldBalanceFunction = "update_hold_balance";
    private const string TriggerPrefix = "tr_";

    private const int ActiveStatusId = (int)HoldStatus.Active;
    private const int SettledStatusId = (int)HoldStatus.Settled;
    private const int ReleasedStatusId = (int)HoldStatus.Released;
    private const int ExpiredStatusId = (int)HoldStatus.Expired;

    private const string HoldBalanceColumn = nameof(AccountEntity.HoldBalance);
    private const string AvailableBalanceColumn = nameof(AccountEntity.AvailableBalance);

    private const string AccountIdColumn = nameof(AccountEntity.AccountId);
    private const string AmountColumn = nameof(HoldEntity.Amount);
    private const string StatusColumn = nameof(HoldEntity.HoldStatusId);

    private const string UpdatedByColumn = nameof(HoldEntity.UpdatedBy);
    private const string UpdatedAtColumn = nameof(HoldEntity.UpdatedAt);
    private const string IsDeletedColumn = nameof(HoldEntity.IsDeleted);
    private const string DeletedByColumn = nameof(HoldEntity.DeletedBy);
    private const string DeletedAtColumn = nameof(HoldEntity.DeletedAt);

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql($@"
            create or replace function {UpdateHoldBalanceFunction}()
            returns trigger as
            $$
            begin
                -- INSERT
                if (TG_OP = 'INSERT') then
                    if (new.""{IsDeletedColumn}"" = false and new.""{StatusColumn}"" = {ActiveStatusId}) then
                        update ""{TableNames.Accounts}""
                        set ""{HoldBalanceColumn}"" = ""{HoldBalanceColumn}"" + new.""{AmountColumn}"",
                            ""{AvailableBalanceColumn}"" = ""{AvailableBalanceColumn}"" - new.""{AmountColumn}"",
                            ""{UpdatedByColumn}"" = new.""{UpdatedByColumn}"",
                            ""{UpdatedAtColumn}"" = new.""{UpdatedAtColumn}""
                        where ""{AccountIdColumn}"" = new.""{AccountIdColumn}""; 
                    end if;
                    return new;
                end if;

                -- UPDATE
                if (TG_OP = 'UPDATE') then
                    -- Account or Amount changed
                    if (old.""{AccountIdColumn}"" <> new.""{AccountIdColumn}"" or old.""{AmountColumn}"" <> new.""{AmountColumn}"") then
                        -- remove old hold from old account
                        if (old.""{StatusColumn}"" = {ActiveStatusId} and old.""{IsDeletedColumn}"" = false) then
                            update ""{TableNames.Accounts}""
                            set ""{HoldBalanceColumn}"" = ""{HoldBalanceColumn}"" - old.""{AmountColumn}"",
                                ""{AvailableBalanceColumn}"" = ""{AvailableBalanceColumn}"" - old.""{AmountColumn}"",
                                ""{UpdatedByColumn}"" = new.""{UpdatedByColumn}"",
                                ""{UpdatedAtColumn}"" = new.""{UpdatedAtColumn}""
                            where ""{AccountIdColumn}"" = old.""{AccountIdColumn}""; 
                        end if;

                        -- add new hold to new account
                        if (new.""{StatusColumn}"" = {ActiveStatusId} and new.""{IsDeletedColumn}"" = false) then
                            update ""{TableNames.Accounts}""
                            set ""{HoldBalanceColumn}"" = ""{HoldBalanceColumn}"" + new.""{AmountColumn}"",
                                ""{AvailableBalanceColumn}"" = ""{AvailableBalanceColumn}"" - new.""{AmountColumn}"",
                                ""{UpdatedByColumn}"" = new.""{UpdatedByColumn}"",
                                ""{UpdatedAtColumn}"" = new.""{UpdatedAtColumn}""
                            where ""{AccountIdColumn}"" = new.""{AccountIdColumn}""; 
                        end if;
                    end if;

                    -- Status change: Active -> Settled/Released/Expired
                    if (old.""{StatusColumn}"" = {ActiveStatusId} and new.""{StatusColumn}"" in ({SettledStatusId}, {ReleasedStatusId}, {ExpiredStatusId})) then
                        update ""{TableNames.Accounts}""
                        set ""{HoldBalanceColumn}"" = ""{HoldBalanceColumn}"" - old.""{AmountColumn}"",
                            ""{AvailableBalanceColumn}"" = ""{AvailableBalanceColumn}"" + old.""{AmountColumn}"",
                            ""{UpdatedByColumn}"" = new.""{UpdatedByColumn}"",
                            ""{UpdatedAtColumn}"" = new.""{UpdatedAtColumn}""
                        where ""{AccountIdColumn}"" = old.""{AccountIdColumn}""; 
                    end if;

                    -- Soft delete
                    if (old.""{StatusColumn}"" = {ActiveStatusId} and old.""{IsDeletedColumn}"" = false and new.""{IsDeletedColumn}"" = true) then
                        update ""{TableNames.Accounts}""
                        set ""{HoldBalanceColumn}"" = ""{HoldBalanceColumn}"" - old.""{AmountColumn}"",
                            ""{AvailableBalanceColumn}"" =  ""{AvailableBalanceColumn}"" - old.""{AmountColumn}"",
                            ""{UpdatedByColumn}"" = new.""{DeletedByColumn}"",
                            ""{UpdatedAtColumn}"" = new.""{DeletedAtColumn}""
                        where ""{AccountIdColumn}"" = old.""{AccountIdColumn}""; 
                    end if;

                    -- Restore from soft delete
                    if (new.""{StatusColumn}"" = {ActiveStatusId} and old.""{IsDeletedColumn}"" = true and new.""{IsDeletedColumn}"" = false) then
                        update ""{TableNames.Accounts}""
                        set ""{HoldBalanceColumn}"" = ""{HoldBalanceColumn}"" + new.""{AmountColumn}"",
                            ""{AvailableBalanceColumn}"" =  ""{AvailableBalanceColumn}"" + new.""{AmountColumn}"",
                            ""{UpdatedAtColumn}"" = new.""{UpdatedAtColumn}""
                        where ""{AccountIdColumn}"" = new.""{AccountIdColumn}""; 
                    end if;

                    return new;
                end if;

                -- DELETE
                if (TG_OP = 'DELETE') then
                    if (old.""{IsDeletedColumn}"" = false and old.""{StatusColumn}"" = {ActiveStatusId}) then
                        update ""{TableNames.Accounts}""
                        set ""{HoldBalanceColumn}"" = ""{HoldBalanceColumn}"" - old.""{AmountColumn}"",
                            ""{AvailableBalanceColumn}"" = ""{AvailableBalanceColumn}"" - old.""{AmountColumn}"",
                            ""{UpdatedAtColumn}"" = now() at time zone 'utc'
                        where ""{AccountIdColumn}"" = old.""{AccountIdColumn}""; 
                    end if;
                    return old;
                end if;

                return null;
            end;
            $$ language plpgsql;
        ");

        migrationBuilder.Sql($@"
            create trigger {TriggerPrefix}{UpdateHoldBalanceFunction}
            after insert or update or delete
            on ""{TableNames.Holds}""
            for each row
            execute function {UpdateHoldBalanceFunction}();
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql($@"drop trigger if exists {TriggerPrefix}{UpdateHoldBalanceFunction} on ""{TableNames.Holds}"";");
        migrationBuilder.Sql($@"drop function if exists {UpdateHoldBalanceFunction};");
    }
}
