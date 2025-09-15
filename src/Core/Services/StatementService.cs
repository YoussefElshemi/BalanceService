using System.Text;
using Core.Constants;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using PageSize = Core.ValueObjects.PageSize;

namespace Core.Services;

public class StatementService(
    IStatementRepository statementRepository,
    TimeProvider timeProvider) : IStatementService
{
    public async Task<Statement> GetAsync(GetStatementRequest getStatementRequest, CancellationToken cancellationToken)
    {
        var openingBalanceRequest = new BalanceRequest
        {
            AccountId = getStatementRequest.AccountId,
            Date = getStatementRequest.DateRange.From
        };

        var closingBalanceRequest = new BalanceRequest
        {
            AccountId = getStatementRequest.AccountId,
            Date = getStatementRequest.DateRange.To
        };

        var openingBalance =
            await statementRepository.GetAvailableBalanceAtAsync(openingBalanceRequest, cancellationToken);
        var closingBalance =
            await statementRepository.GetAvailableBalanceAtAsync(closingBalanceRequest, cancellationToken);

        var count = await statementRepository.CountAsync(getStatementRequest, cancellationToken);
        var statementEntries = await statementRepository.QueryAsync(getStatementRequest, cancellationToken);

        return new Statement
        {
            AccountId = getStatementRequest.AccountId,
            DateRange = getStatementRequest.DateRange,
            OpeningBalance = openingBalance,
            ClosingBalance = closingBalance,
            StatementEntries = new PagedResults<StatementEntry>
            {
                Data = statementEntries,
                MetaData = new PagedMetadata
                {
                    TotalRecords = count,
                    TotalPages = (count + getStatementRequest.PageSize - 1) / getStatementRequest.PageSize,
                    PageSize = getStatementRequest.PageSize,
                    PageNumber = getStatementRequest.PageNumber
                }
            }
        };
    }

    public async Task<byte[]> GeneratePdfAsync(GenerateStatementRequest generateStatementRequest, CancellationToken cancellationToken)
    {
        var openingBalanceRequest = new BalanceRequest
        {
            AccountId = generateStatementRequest.AccountId,
            Date = generateStatementRequest.DateRange.From
        };

        var closingBalanceRequest = new BalanceRequest
        {
            AccountId = generateStatementRequest.AccountId,
            Date = generateStatementRequest.DateRange.To
        };

        var openingBalance = await statementRepository.GetAvailableBalanceAtAsync(openingBalanceRequest, cancellationToken);
        var closingBalance = await statementRepository.GetAvailableBalanceAtAsync(closingBalanceRequest, cancellationToken);

        var getStatementRequest = new GetStatementRequest
        {
            PageSize = new PageSize(0),
            PageNumber = new PageNumber(0),
            AccountId = generateStatementRequest.AccountId,
            DateRange = generateStatementRequest.DateRange,
            Direction = generateStatementRequest.Direction,
        };

        var allEntries = await statementRepository.QueryAllAsync(getStatementRequest, cancellationToken);

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);

                page.Header().PaddingBottom(20).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item()
                            .Text("Account Statement")
                            .FontSize(22)
                            .SemiBold()
                            .FontColor(Colors.Blue.Medium);

                        col.Item()
                            .Text($"Account: {generateStatementRequest.AccountId}")
                            .FontSize(10)
                            .FontColor(Colors.Grey.Darken2);

                        col.Item()
                            .Text($"Period: {generateStatementRequest.DateRange.From.ToString(DateTimeConstants.DateFormat)} → {generateStatementRequest.DateRange.To.ToString(DateTimeConstants.DateFormat)}")
                            .FontSize(10).FontColor(Colors.Grey.Darken2);
                    });

                    row.ConstantItem(80)
                        .Height(40)
                        .Background(Colors.Blue.Medium)
                        .AlignCenter()
                        .AlignMiddle()
                        .Text("Balance Service")
                        .FontColor(Colors.White)
                        .SemiBold();
                });

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Background(Colors.Grey.Lighten3).Padding(10).Column(summary =>
                        {
                            summary.Item()
                                .Text("Opening Balance")
                                .FontSize(10)
                                .FontColor(Colors.Grey.Darken2);

                            summary.Item()
                                .Text($"{(decimal)openingBalance:0.00}")
                                .FontSize(14)
                                .SemiBold()
                                .FontColor(Colors.Green.Darken2);
                        });

                        row.RelativeItem().Background(Colors.Grey.Lighten3).Padding(10).Column(summary =>
                        {
                            summary.Item()
                                .Text("Closing Balance")
                                .FontSize(10)
                                .FontColor(Colors.Grey.Darken2);
           
                            summary.Item()
                                .Text($"{(decimal)closingBalance:0.00}")
                                .FontSize(14)
                                .SemiBold()
                                .FontColor(Colors.Red.Darken2);
                        });
                    });

                    col.Item()
                        .LineHorizontal(1)
                        .LineColor(Colors.Grey.Lighten2);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(90);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell()
                                .Background(Colors.Blue.Lighten3)
                                .Padding(5)
                                .Text("Date")
                                .SemiBold();

                            header.Cell()
                                .Background(Colors.Blue.Lighten3)
                                .Padding(5)
                                .Text("Description")
                                .SemiBold();
                            
                            header.Cell()
                                .Background(Colors.Blue.Lighten3)
                                .Padding(5)
                                .Text("Reference")
                                .SemiBold();

                            header.Cell()
                                .Background(Colors.Blue.Lighten3)
                                .Padding(5)
                                .AlignRight()
                                .Text("Amount")
                                .SemiBold();

                            header.Cell()
                                .Background(Colors.Blue.Lighten3)
                                .Padding(5)
                                .AlignRight()
                                .Text("Balance")
                                .SemiBold();
                        });

                        foreach (var entry in allEntries)
                        {
                            var isCredit = entry.Direction == StatementDirection.Credit;
                            var amount = (isCredit ? "+" : "-") + $"{(decimal)entry.Amount:0.00}";
                            var amountColour = isCredit ? Colors.Green.Darken2 : Colors.Red.Darken2;
                            var balance = $"{(decimal)entry.AvailableBalance:0.00}";
               
                            table.Cell()
                                .Padding(5)
                                .Text(entry.Date.ToString())
                                .FontSize(10);

                            table.Cell()
                                .Padding(5)
                                .Text(entry.Description)
                                .FontSize(10);
                            
                            table.Cell()
                                .Padding(5)
                                .Text(entry.Reference)
                                .FontSize(10);
                            
                            table.Cell()
                                .Padding(5)
                                .AlignRight()
                                .Text(amount)
                                .FontSize(10)
                                .FontColor(amountColour);
                            
                            table.Cell()
                                .Padding(5)
                                .AlignRight()
                                .Text(balance)
                                .FontSize(10);
                        }
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Generated at ")
                        .FontSize(9)
                        .FontColor(Colors.Grey.Darken1);
      
                    x.Span($"{timeProvider.GetUtcNow().ToString(DateTimeConstants.DateTimeFormat)} UTC")
                        .FontSize(9)
                        .FontColor(Colors.Grey.Darken1);
                });
            });
        }).GeneratePdf();

        return pdf;
    }
    
    public async Task<byte[]> GenerateCsvAsync(GenerateStatementRequest generateCsvStatementRequest, CancellationToken cancellationToken)
    {
        var allEntries = await GetStatementEntriesAsync(generateCsvStatementRequest, cancellationToken);

        var csv = new StringBuilder();

        csv.AppendLine("Date,Description,Reference,Direction,Type,Amount,Balance");

        foreach (var entry in allEntries)
        {
            var values = new List<string?>()
            {
                entry.Date.ToString(),
                CsvEscape(entry.Description),
                CsvEscape(entry.Reference),
                entry.Direction.ToString(),
                entry.Type.ToString(),
                $"{(decimal)entry.Amount:0.00}",
                $"{(decimal)entry.AvailableBalance:0.00}"
            };
            
            csv.AppendLine(string.Join(',', values));
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    private  Task<List<StatementEntry>> GetStatementEntriesAsync(GenerateStatementRequest generateCsvStatementRequest, CancellationToken cancellationToken)
    {
        var getStatementRequest = new GetStatementRequest
        {
            PageSize = new PageSize(0),
            PageNumber = new PageNumber(0),
            AccountId = generateCsvStatementRequest.AccountId,
            DateRange = generateCsvStatementRequest.DateRange,
            Direction = generateCsvStatementRequest.Direction,
        };

        return statementRepository.QueryAllAsync(getStatementRequest, cancellationToken);
    }

    private static string CsvEscape(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        if (value.Contains(',') || value.Contains('"'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}