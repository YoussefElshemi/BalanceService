using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessedAtFieldsToHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "TransactionHistory",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ProcessedAt",
                table: "TransactionHistory",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "HoldHistory",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ProcessedAt",
                table: "HoldHistory",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "AccountHistory",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ProcessedAt",
                table: "AccountHistory",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "TransactionHistory");

            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                table: "TransactionHistory");

            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "HoldHistory");

            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                table: "HoldHistory");

            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "AccountHistory");

            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                table: "AccountHistory");
        }
    }
}
