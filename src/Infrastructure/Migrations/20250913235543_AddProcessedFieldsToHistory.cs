using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessedFieldsToHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ProcessedAt",
                table: "TransactionHistory",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcessingStatusId",
                table: "TransactionHistory",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "TransactionHistory",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ProcessedAt",
                table: "HoldHistory",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcessingStatusId",
                table: "HoldHistory",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "HoldHistory",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ProcessedAt",
                table: "AccountHistory",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcessingStatusId",
                table: "AccountHistory",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "AccountHistory",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateTable(
                name: "ProcessingStatuses",
                columns: table => new
                {
                    ProcessingStatusId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessingStatuses", x => x.ProcessingStatusId);
                });

            migrationBuilder.InsertData(
                table: "ProcessingStatuses",
                columns: new[] { "ProcessingStatusId", "Name" },
                values: new object[,]
                {
                    { 1, "NotProcessed" },
                    { 2, "Processing" },
                    { 3, "Processed" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistory_ProcessingStatusId",
                table: "TransactionHistory",
                column: "ProcessingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_HoldHistory_ProcessingStatusId",
                table: "HoldHistory",
                column: "ProcessingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHistory_ProcessingStatusId",
                table: "AccountHistory",
                column: "ProcessingStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountHistory_ProcessingStatuses_ProcessingStatusId",
                table: "AccountHistory",
                column: "ProcessingStatusId",
                principalTable: "ProcessingStatuses",
                principalColumn: "ProcessingStatusId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HoldHistory_ProcessingStatuses_ProcessingStatusId",
                table: "HoldHistory",
                column: "ProcessingStatusId",
                principalTable: "ProcessingStatuses",
                principalColumn: "ProcessingStatusId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistory_ProcessingStatuses_ProcessingStatusId",
                table: "TransactionHistory",
                column: "ProcessingStatusId",
                principalTable: "ProcessingStatuses",
                principalColumn: "ProcessingStatusId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountHistory_ProcessingStatuses_ProcessingStatusId",
                table: "AccountHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_HoldHistory_ProcessingStatuses_ProcessingStatusId",
                table: "HoldHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistory_ProcessingStatuses_ProcessingStatusId",
                table: "TransactionHistory");

            migrationBuilder.DropTable(
                name: "ProcessingStatuses");

            migrationBuilder.DropIndex(
                name: "IX_TransactionHistory_ProcessingStatusId",
                table: "TransactionHistory");

            migrationBuilder.DropIndex(
                name: "IX_HoldHistory_ProcessingStatusId",
                table: "HoldHistory");

            migrationBuilder.DropIndex(
                name: "IX_AccountHistory_ProcessingStatusId",
                table: "AccountHistory");

            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                table: "TransactionHistory");

            migrationBuilder.DropColumn(
                name: "ProcessingStatusId",
                table: "TransactionHistory");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "TransactionHistory");

            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                table: "HoldHistory");

            migrationBuilder.DropColumn(
                name: "ProcessingStatusId",
                table: "HoldHistory");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "HoldHistory");

            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                table: "AccountHistory");

            migrationBuilder.DropColumn(
                name: "ProcessingStatusId",
                table: "AccountHistory");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "AccountHistory");
        }
    }
}
