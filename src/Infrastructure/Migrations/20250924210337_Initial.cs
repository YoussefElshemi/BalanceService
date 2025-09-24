using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountStatuses",
                columns: table => new
                {
                    AccountStatusId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountStatuses", x => x.AccountStatusId);
                });

            migrationBuilder.CreateTable(
                name: "AccountTypes",
                columns: table => new
                {
                    AccountTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTypes", x => x.AccountTypeId);
                });

            migrationBuilder.CreateTable(
                name: "HistoryTypes",
                columns: table => new
                {
                    HistoryTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryTypes", x => x.HistoryTypeId);
                });

            migrationBuilder.CreateTable(
                name: "HoldSources",
                columns: table => new
                {
                    HoldSourceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoldSources", x => x.HoldSourceId);
                });

            migrationBuilder.CreateTable(
                name: "HoldStatuses",
                columns: table => new
                {
                    HoldStatusId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoldStatuses", x => x.HoldStatusId);
                });

            migrationBuilder.CreateTable(
                name: "HoldTypes",
                columns: table => new
                {
                    HoldTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoldTypes", x => x.HoldTypeId);
                });

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
                name: "Jobs",
                columns: table => new
                {
                    JobId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobName = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_Jobs", x => x.JobId);
                });

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

            migrationBuilder.CreateTable(
                name: "TransactionDirections",
                columns: table => new
                {
                    TransactionDirectionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionDirections", x => x.TransactionDirectionId);
                });

            migrationBuilder.CreateTable(
                name: "TransactionSources",
                columns: table => new
                {
                    TransactionSourceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionSources", x => x.TransactionSourceId);
                });

            migrationBuilder.CreateTable(
                name: "TransactionStatuses",
                columns: table => new
                {
                    TransactionStatusId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionStatuses", x => x.TransactionStatusId);
                });

            migrationBuilder.CreateTable(
                name: "TransactionTypes",
                columns: table => new
                {
                    TransactionTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTypes", x => x.TransactionTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountName = table.Column<string>(type: "text", nullable: false),
                    CurrencyCode = table.Column<string>(type: "text", nullable: false),
                    LedgerBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    AvailableBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    PendingDebitBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    PendingCreditBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    HoldBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    MinimumRequiredBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    AccountTypeId = table.Column<int>(type: "integer", nullable: false),
                    AccountStatusId = table.Column<int>(type: "integer", nullable: false),
                    Metadata = table.Column<string>(type: "jsonb", nullable: true),
                    ParentAccountId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_Accounts_AccountStatuses_AccountStatusId",
                        column: x => x.AccountStatusId,
                        principalTable: "AccountStatuses",
                        principalColumn: "AccountStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Accounts_AccountTypes_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalTable: "AccountTypes",
                        principalColumn: "AccountTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Accounts_Accounts_ParentAccountId",
                        column: x => x.ParentAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.SetNull);
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
                name: "JobRuns",
                columns: table => new
                {
                    JobRunId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsExecuted = table.Column<bool>(type: "boolean", nullable: false),
                    ExecutedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
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
                    table.PrimaryKey("PK_JobRuns", x => x.JobRunId);
                    table.ForeignKey(
                        name: "FK_JobRuns_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountHistory",
                columns: table => new
                {
                    AccountHistoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    HistoryTypeId = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountName = table.Column<string>(type: "text", nullable: false),
                    CurrencyCode = table.Column<string>(type: "text", nullable: false),
                    LedgerBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    AvailableBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    PendingDebitBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    PendingCreditBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    HoldBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    MinimumRequiredBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    AccountTypeId = table.Column<int>(type: "integer", nullable: false),
                    AccountStatusId = table.Column<int>(type: "integer", nullable: false),
                    Metadata = table.Column<string>(type: "jsonb", nullable: true),
                    ParentAccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    ProcessingStatusId = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    ProcessedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountHistory", x => x.AccountHistoryId);
                    table.ForeignKey(
                        name: "FK_AccountHistory_AccountStatuses_AccountStatusId",
                        column: x => x.AccountStatusId,
                        principalTable: "AccountStatuses",
                        principalColumn: "AccountStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountHistory_AccountTypes_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalTable: "AccountTypes",
                        principalColumn: "AccountTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountHistory_Accounts_ParentAccountId",
                        column: x => x.ParentAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AccountHistory_HistoryTypes_HistoryTypeId",
                        column: x => x.HistoryTypeId,
                        principalTable: "HistoryTypes",
                        principalColumn: "HistoryTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountHistory_ProcessingStatuses_ProcessingStatusId",
                        column: x => x.ProcessingStatusId,
                        principalTable: "ProcessingStatuses",
                        principalColumn: "ProcessingStatusId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransactionHistory",
                columns: table => new
                {
                    TransactionHistoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    HistoryTypeId = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrencyCode = table.Column<string>(type: "text", nullable: false),
                    TransactionDirectionId = table.Column<int>(type: "integer", nullable: false),
                    PostedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IdempotencyKey = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionTypeId = table.Column<int>(type: "integer", nullable: false),
                    TransactionStatusId = table.Column<int>(type: "integer", nullable: false),
                    TransactionSourceId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Reference = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    ProcessingStatusId = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    ProcessedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionHistory", x => x.TransactionHistoryId);
                    table.ForeignKey(
                        name: "FK_TransactionHistory_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionHistory_HistoryTypes_HistoryTypeId",
                        column: x => x.HistoryTypeId,
                        principalTable: "HistoryTypes",
                        principalColumn: "HistoryTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionHistory_ProcessingStatuses_ProcessingStatusId",
                        column: x => x.ProcessingStatusId,
                        principalTable: "ProcessingStatuses",
                        principalColumn: "ProcessingStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionHistory_TransactionDirections_TransactionDirecti~",
                        column: x => x.TransactionDirectionId,
                        principalTable: "TransactionDirections",
                        principalColumn: "TransactionDirectionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionHistory_TransactionSources_TransactionSourceId",
                        column: x => x.TransactionSourceId,
                        principalTable: "TransactionSources",
                        principalColumn: "TransactionSourceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionHistory_TransactionStatuses_TransactionStatusId",
                        column: x => x.TransactionStatusId,
                        principalTable: "TransactionStatuses",
                        principalColumn: "TransactionStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionHistory_TransactionTypes_TransactionTypeId",
                        column: x => x.TransactionTypeId,
                        principalTable: "TransactionTypes",
                        principalColumn: "TransactionTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrencyCode = table.Column<string>(type: "text", nullable: false),
                    TransactionDirectionId = table.Column<int>(type: "integer", nullable: false),
                    PostedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IdempotencyKey = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionTypeId = table.Column<int>(type: "integer", nullable: false),
                    TransactionStatusId = table.Column<int>(type: "integer", nullable: false),
                    TransactionSourceId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Reference = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_TransactionDirections_TransactionDirectionId",
                        column: x => x.TransactionDirectionId,
                        principalTable: "TransactionDirections",
                        principalColumn: "TransactionDirectionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_TransactionSources_TransactionSourceId",
                        column: x => x.TransactionSourceId,
                        principalTable: "TransactionSources",
                        principalColumn: "TransactionSourceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_TransactionStatuses_TransactionStatusId",
                        column: x => x.TransactionStatusId,
                        principalTable: "TransactionStatuses",
                        principalColumn: "TransactionStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_TransactionTypes_TransactionTypeId",
                        column: x => x.TransactionTypeId,
                        principalTable: "TransactionTypes",
                        principalColumn: "TransactionTypeId",
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
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
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
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    ProcessingStatusId = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    ProcessedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
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
                        name: "FK_HoldHistory_ProcessingStatuses_ProcessingStatusId",
                        column: x => x.ProcessingStatusId,
                        principalTable: "ProcessingStatuses",
                        principalColumn: "ProcessingStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoldHistory_Transactions_SettledTransactionId",
                        column: x => x.SettledTransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Holds",
                columns: table => new
                {
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
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holds", x => x.HoldId);
                    table.ForeignKey(
                        name: "FK_Holds_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Holds_HoldSources_HoldSourceId",
                        column: x => x.HoldSourceId,
                        principalTable: "HoldSources",
                        principalColumn: "HoldSourceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Holds_HoldStatuses_HoldStatusId",
                        column: x => x.HoldStatusId,
                        principalTable: "HoldStatuses",
                        principalColumn: "HoldStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Holds_HoldTypes_HoldTypeId",
                        column: x => x.HoldTypeId,
                        principalTable: "HoldTypes",
                        principalColumn: "HoldTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Holds_Transactions_SettledTransactionId",
                        column: x => x.SettledTransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "AccountStatuses",
                columns: new[] { "AccountStatusId", "Name" },
                values: new object[,]
                {
                    { 1, "Active" },
                    { 2, "Frozen" },
                    { 3, "Closed" },
                    { 4, "PendingActivation" },
                    { 5, "Dormant" },
                    { 6, "Suspended" },
                    { 7, "Restricted" },
                    { 8, "PendingClosure" }
                });

            migrationBuilder.InsertData(
                table: "AccountTypes",
                columns: new[] { "AccountTypeId", "Name" },
                values: new object[,]
                {
                    { 1, "Asset" },
                    { 2, "Liability" },
                    { 3, "Equity" },
                    { 4, "Revenue" },
                    { 5, "Expense" }
                });

            migrationBuilder.InsertData(
                table: "HistoryTypes",
                columns: new[] { "HistoryTypeId", "Name" },
                values: new object[,]
                {
                    { 1, "Insert" },
                    { 2, "Modify" },
                    { 3, "Delete" }
                });

            migrationBuilder.InsertData(
                table: "HoldSources",
                columns: new[] { "HoldSourceId", "Name" },
                values: new object[,]
                {
                    { 1, "Api" },
                    { 2, "Import" },
                    { 3, "Manual" }
                });

            migrationBuilder.InsertData(
                table: "HoldStatuses",
                columns: new[] { "HoldStatusId", "Name" },
                values: new object[,]
                {
                    { 1, "Active" },
                    { 2, "Released" },
                    { 3, "Settled" },
                    { 4, "Expired" }
                });

            migrationBuilder.InsertData(
                table: "HoldTypes",
                columns: new[] { "HoldTypeId", "Name" },
                values: new object[,]
                {
                    { 1, "CardAuthorization" },
                    { 2, "CheckDeposit" },
                    { 3, "Merchant" },
                    { 4, "Regulatory" }
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
                table: "ProcessingStatuses",
                columns: new[] { "ProcessingStatusId", "Name" },
                values: new object[,]
                {
                    { 1, "NotProcessed" },
                    { 2, "Processing" },
                    { 3, "Processed" }
                });

            migrationBuilder.InsertData(
                table: "TransactionDirections",
                columns: new[] { "TransactionDirectionId", "Name" },
                values: new object[,]
                {
                    { 1, "Credit" },
                    { 2, "Debit" }
                });

            migrationBuilder.InsertData(
                table: "TransactionSources",
                columns: new[] { "TransactionSourceId", "Name" },
                values: new object[,]
                {
                    { 1, "Api" },
                    { 2, "Import" },
                    { 3, "Manual" },
                    { 4, "Internal" }
                });

            migrationBuilder.InsertData(
                table: "TransactionStatuses",
                columns: new[] { "TransactionStatusId", "Name" },
                values: new object[,]
                {
                    { 1, "Draft" },
                    { 2, "Posted" },
                    { 3, "Reversed" }
                });

            migrationBuilder.InsertData(
                table: "TransactionTypes",
                columns: new[] { "TransactionTypeId", "Name" },
                values: new object[,]
                {
                    { 1, "InboundFunds" },
                    { 2, "SettledHold" },
                    { 3, "Transfer" },
                    { 4, "AccruedInterest" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountHistory_AccountId",
                table: "AccountHistory",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHistory_AccountStatusId",
                table: "AccountHistory",
                column: "AccountStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHistory_AccountTypeId",
                table: "AccountHistory",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHistory_HistoryTypeId",
                table: "AccountHistory",
                column: "HistoryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHistory_ParentAccountId",
                table: "AccountHistory",
                column: "ParentAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHistory_ProcessingStatusId",
                table: "AccountHistory",
                column: "ProcessingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountStatusId",
                table: "Accounts",
                column: "AccountStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountTypeId",
                table: "Accounts",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ParentAccountId",
                table: "Accounts",
                column: "ParentAccountId");

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
                name: "IX_HoldHistory_ProcessingStatusId",
                table: "HoldHistory",
                column: "ProcessingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_HoldHistory_SettledTransactionId",
                table: "HoldHistory",
                column: "SettledTransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Holds_AccountId",
                table: "Holds",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Holds_HoldSourceId",
                table: "Holds",
                column: "HoldSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Holds_HoldStatusId",
                table: "Holds",
                column: "HoldStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Holds_HoldTypeId",
                table: "Holds",
                column: "HoldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Holds_IdempotencyKey",
                table: "Holds",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Holds_SettledTransactionId",
                table: "Holds",
                column: "SettledTransactionId",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_JobRuns_JobId_ScheduledAt",
                table: "JobRuns",
                columns: new[] { "JobId", "ScheduledAt" },
                unique: true,
                filter: "\"IsDeleted\" = FALSE");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_JobName",
                table: "Jobs",
                column: "JobName",
                unique: true,
                filter: "\"IsDeleted\" = FALSE");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistory_AccountId",
                table: "TransactionHistory",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistory_HistoryTypeId",
                table: "TransactionHistory",
                column: "HistoryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistory_ProcessingStatusId",
                table: "TransactionHistory",
                column: "ProcessingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistory_TransactionDirectionId",
                table: "TransactionHistory",
                column: "TransactionDirectionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistory_TransactionId",
                table: "TransactionHistory",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistory_TransactionSourceId",
                table: "TransactionHistory",
                column: "TransactionSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistory_TransactionStatusId",
                table: "TransactionHistory",
                column: "TransactionStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistory_TransactionTypeId",
                table: "TransactionHistory",
                column: "TransactionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId",
                table: "Transactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_IdempotencyKey",
                table: "Transactions",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionDirectionId",
                table: "Transactions",
                column: "TransactionDirectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionSourceId",
                table: "Transactions",
                column: "TransactionSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionStatusId",
                table: "Transactions",
                column: "TransactionStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionTypeId",
                table: "Transactions",
                column: "TransactionTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountHistory");

            migrationBuilder.DropTable(
                name: "HoldHistory");

            migrationBuilder.DropTable(
                name: "Holds");

            migrationBuilder.DropTable(
                name: "InterestAccruals");

            migrationBuilder.DropTable(
                name: "InterestProductAccountLinks");

            migrationBuilder.DropTable(
                name: "JobRuns");

            migrationBuilder.DropTable(
                name: "TransactionHistory");

            migrationBuilder.DropTable(
                name: "HoldSources");

            migrationBuilder.DropTable(
                name: "HoldStatuses");

            migrationBuilder.DropTable(
                name: "HoldTypes");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "InterestProducts");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "HistoryTypes");

            migrationBuilder.DropTable(
                name: "ProcessingStatuses");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "TransactionDirections");

            migrationBuilder.DropTable(
                name: "TransactionSources");

            migrationBuilder.DropTable(
                name: "TransactionStatuses");

            migrationBuilder.DropTable(
                name: "TransactionTypes");

            migrationBuilder.DropTable(
                name: "InterestPayoutFrequencies");

            migrationBuilder.DropTable(
                name: "AccountStatuses");

            migrationBuilder.DropTable(
                name: "AccountTypes");
        }
    }
}
