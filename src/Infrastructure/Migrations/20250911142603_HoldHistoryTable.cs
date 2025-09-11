using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HoldHistoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HoldHistory",
                columns: table => new
                {
                    HoldHistoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    HistoryTypeId = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    HoldId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrencyCode = table.Column<string>(type: "text", nullable: false),
                    IdempotencyKey = table.Column<Guid>(type: "uuid", nullable: false),
                    HoldTypeId = table.Column<int>(type: "integer", nullable: false),
                    HoldStatusId = table.Column<int>(type: "integer", nullable: false),
                    HoldSourceId = table.Column<int>(type: "integer", nullable: false),
                    SettledTransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Reference = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoldHistory", x => x.HoldHistoryId);
                    table.ForeignKey(
                        name: "FK_HoldHistory_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoldHistory_HistoryTypes_HistoryTypeId",
                        column: x => x.HistoryTypeId,
                        principalTable: "HistoryTypes",
                        principalColumn: "HistoryTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoldHistory_HoldSources_HoldSourceId",
                        column: x => x.HoldSourceId,
                        principalTable: "HoldSources",
                        principalColumn: "HoldSourceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoldHistory_HoldStatuses_HoldStatusId",
                        column: x => x.HoldStatusId,
                        principalTable: "HoldStatuses",
                        principalColumn: "HoldStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoldHistory_HoldTypes_HoldTypeId",
                        column: x => x.HoldTypeId,
                        principalTable: "HoldTypes",
                        principalColumn: "HoldTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoldHistory_Transactions_SettledTransactionId",
                        column: x => x.SettledTransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoldHistory_AccountId",
                table: "HoldHistory",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_HoldHistory_HistoryTypeId",
                table: "HoldHistory",
                column: "HistoryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HoldHistory_HoldId",
                table: "HoldHistory",
                column: "HoldId");

            migrationBuilder.CreateIndex(
                name: "IX_HoldHistory_HoldSourceId",
                table: "HoldHistory",
                column: "HoldSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_HoldHistory_HoldStatusId",
                table: "HoldHistory",
                column: "HoldStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_HoldHistory_HoldTypeId",
                table: "HoldHistory",
                column: "HoldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HoldHistory_SettledTransactionId",
                table: "HoldHistory",
                column: "SettledTransactionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoldHistory");
        }
    }
}
