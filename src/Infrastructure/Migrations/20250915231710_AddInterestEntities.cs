using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInterestEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InterestPayoutFrequencies",
                columns: table => new
                {
                    InterestPayoutFrequencyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestPayoutFrequencies", x => x.InterestPayoutFrequencyId);
                });

            migrationBuilder.CreateTable(
                name: "InterestProducts",
                columns: table => new
                {
                    InterestProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AnnualInterestRate = table.Column<decimal>(type: "numeric", nullable: false),
                    InterestPayoutFrequencyId = table.Column<int>(type: "integer", nullable: false),
                    AccrualBasis = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestProducts", x => x.InterestProductId);
                    table.ForeignKey(
                        name: "FK_InterestProducts_InterestPayoutFrequencies_InterestPayoutFr~",
                        column: x => x.InterestPayoutFrequencyId,
                        principalTable: "InterestPayoutFrequencies",
                        principalColumn: "InterestPayoutFrequencyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterestAccruals",
                columns: table => new
                {
                    InterestAccrualId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    InterestProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    DailyInterestRate = table.Column<decimal>(type: "numeric", nullable: false),
                    AccruedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AccruedAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    IsPosted = table.Column<bool>(type: "boolean", nullable: false),
                    PostedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestAccruals", x => x.InterestAccrualId);
                    table.ForeignKey(
                        name: "FK_InterestAccruals_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterestAccruals_InterestProducts_InterestProductId",
                        column: x => x.InterestProductId,
                        principalTable: "InterestProducts",
                        principalColumn: "InterestProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterestProductAccountLinks",
                columns: table => new
                {
                    InterestProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestProductAccountLinks", x => new { x.AccountId, x.InterestProductId });
                    table.ForeignKey(
                        name: "FK_InterestProductAccountLinks_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterestProductAccountLinks_InterestProducts_InterestProduc~",
                        column: x => x.InterestProductId,
                        principalTable: "InterestProducts",
                        principalColumn: "InterestProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "InterestPayoutFrequencies",
                columns: new[] { "InterestPayoutFrequencyId", "Name" },
                values: new object[,]
                {
                    { 1, "Daily" },
                    { 2, "Weekly" },
                    { 3, "Monthly" },
                    { 4, "Quarterly" },
                    { 5, "Yearly" }
                });

            migrationBuilder.InsertData(
                table: "TransactionSources",
                columns: new[] { "TransactionSourceId", "Name" },
                values: new object[] { 4, "Internal" });

            migrationBuilder.InsertData(
                table: "TransactionTypes",
                columns: new[] { "TransactionTypeId", "Name" },
                values: new object[] { 4, "AccruedInterest" });

            migrationBuilder.CreateIndex(
                name: "IX_InterestAccruals_AccountId",
                table: "InterestAccruals",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestAccruals_InterestProductId",
                table: "InterestAccruals",
                column: "InterestProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestProductAccountLinks_InterestProductId",
                table: "InterestProductAccountLinks",
                column: "InterestProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestProducts_InterestPayoutFrequencyId",
                table: "InterestProducts",
                column: "InterestPayoutFrequencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterestAccruals");

            migrationBuilder.DropTable(
                name: "InterestProductAccountLinks");

            migrationBuilder.DropTable(
                name: "InterestProducts");

            migrationBuilder.DropTable(
                name: "InterestPayoutFrequencies");

            migrationBuilder.DeleteData(
                table: "TransactionSources",
                keyColumn: "TransactionSourceId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TransactionTypes",
                keyColumn: "TransactionTypeId",
                keyValue: 4);
        }
    }
}
