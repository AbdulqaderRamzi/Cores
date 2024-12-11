using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Cores.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Cores.Web.Areas.Accounting.Controllers;

[Area("Accounting")]
[Authorize(Roles = SD.ACCOUNTING_ROLE + "," + SD.ADMIN_ROLE)]
public class DashboardController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _db;

    public DashboardController(IUnitOfWork unitOfWork, ApplicationDbContext db)
    {
        _unitOfWork = unitOfWork;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var totalAssets = await _unitOfWork.Account.GetAll(a => a.Type == "Asset" && a.IsActive);
        var totalLiabilities = await _unitOfWork.Account.GetAll(a => a.Type == "Liability" && a.IsActive);
        var totalRevenue = await _unitOfWork.Account.GetAll(a => a.Type == "Revenue" && a.IsActive);
        var totalExpenses = await _unitOfWork.Account.GetAll(a => a.Type == "Expense" && a.IsActive);
        var recentTransactions = await _unitOfWork.Transaction.GetAll(
            t => t.Status == "Posted",
            includeProperties: "Details"
        );

        var totalAssetsValue = totalAssets.Sum(a => a.Balance);
        var totalLiabilitiesValue = totalLiabilities.Sum(l => l.Balance);
        var totalRevenueValue = totalRevenue.Sum(r => r.Balance);
        var totalExpensesValue = totalExpenses.Sum(e => e.Balance);

        var dashboard = new AccountingDashboard
        {
            TotalAssets = totalAssetsValue,
            TotalLiabilities = totalLiabilitiesValue,
            TotalEquity = totalAssetsValue - totalLiabilitiesValue, // Added this line
            TotalRevenue = totalRevenueValue,
            TotalExpenses = totalExpensesValue,
            NetIncome = totalRevenueValue - totalExpensesValue,
            RecentTransactions = recentTransactions,
            MonthlyRevenue = await GetMonthlyRevenue(),
            MonthlyExpenses = await GetMonthlyExpenses(),
            AccountTypeDistribution = await GetAccountTypeDistribution()
        };

        return View(dashboard);
    }

    private async Task<Dictionary<string, decimal>> GetMonthlyRevenue()
    {
        var oneYearAgo = DateTime.Now.AddYears(-1);

        var transactions = await _db.Transactions
            .Where(t => t.TransactionDate >= oneYearAgo && t.Status == "Posted")
            .Include(t => t.Details)
            .ThenInclude(d => d.Account)
            .SelectMany(t => t.Details)
            .Where(d => d.Account.Type == "Revenue")
            .GroupBy(p => new { p.Transaction.TransactionDate.Year, p.Transaction.TransactionDate.Month })
            .Select(g => new
            {
                YearMonth = new DateTime(g.Key.Year, g.Key.Month, 1),
                Total = g.Sum(d => d.CreditAmount - d.DebitAmount)
            })
            .ToDictionaryAsync(
                k => k.YearMonth.ToString("MMM"),
                v => v.Total
            );

        var allMonths = new[] 
        { 
            "Jan", "Feb", "Mar", "Apr", "May", "Jun", 
            "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        };
        
        return allMonths.ToDictionary(
            month => month,
            month => transactions.TryGetValue(month, out var value) ? value : 0m
        );
        
    }

    private async Task<Dictionary<string, decimal>> GetMonthlyExpenses()
    {
        var oneYearAgo = DateTime.Now.AddYears(-1);
        
        var transactions = await _db.Transactions
            .Where(t => t.TransactionDate >= oneYearAgo && t.Status == "Posted")
            .Include(t => t.Details)
            .ThenInclude(d => d.Account)
            .SelectMany(t => t.Details)
            .Where(d => d.Account.Type == "Expense")
            .GroupBy(p => new { p.Transaction.TransactionDate.Year, p.Transaction.TransactionDate.Month })
            .Select(g => new
            {
                YearMonth = new DateTime(g.Key.Year, g.Key.Month, 1),
                Total = g.Sum(d => d.DebitAmount - d.CreditAmount)
            })
            .ToDictionaryAsync(
                k => k.YearMonth.ToString("MMM"),
                v => v.Total
            );

        var allMonths = new[] 
        { 
            "Jan", "Feb", "Mar", "Apr", "May", "Jun", 
            "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        };
        
        return allMonths.ToDictionary(
            month => month,
            month => transactions.TryGetValue(month, out var value) ? value : 0m
        );
    }

    private async Task<Dictionary<string, decimal>> GetAccountTypeDistribution()
    {
        var accounts = await _unitOfWork.Account.GetAll(a => a.IsActive);
        return accounts
            .GroupBy(a => a.Type)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(a => Math.Abs(a.Balance))
            );
    }
    
    [HttpGet]
    public async Task<IActionResult> GenerateReport()
    {
        var totalAssets = await _unitOfWork.Account.GetAll(a => a.Type == "Asset" && a.IsActive);
        var totalLiabilities = await _unitOfWork.Account.GetAll(a => a.Type == "Liability" && a.IsActive);
        var totalRevenue = await _unitOfWork.Account.GetAll(a => a.Type == "Revenue" && a.IsActive);
        var totalExpenses = await _unitOfWork.Account.GetAll(a => a.Type == "Expense" && a.IsActive);
        var recentTransactions = await _unitOfWork.Transaction.GetAll(
            t => t.Status == "Posted",
            includeProperties: "Details"
        );

        var totalAssetsValue = totalAssets.Sum(a => a.Balance);
        var totalLiabilitiesValue = totalLiabilities.Sum(l => l.Balance);
        var totalRevenueValue = totalRevenue.Sum(r => r.Balance);
        var totalExpensesValue = totalExpenses.Sum(e => e.Balance);

        var dashboard = new AccountingDashboard
        {
            TotalAssets = totalAssetsValue,
            TotalLiabilities = totalLiabilitiesValue,
            TotalEquity = totalAssetsValue - totalLiabilitiesValue, // Added this line
            TotalRevenue = totalRevenueValue,
            TotalExpenses = totalExpensesValue,
            NetIncome = totalRevenueValue - totalExpensesValue,
            RecentTransactions = recentTransactions,
            MonthlyRevenue = await GetMonthlyRevenue(),
            MonthlyExpenses = await GetMonthlyExpenses(),
            AccountTypeDistribution = await GetAccountTypeDistribution()
        };

        var pdfData = GenerateAccountingReport(dashboard);
        return File(pdfData, "application/pdf", $"Accounting_Dashboard_Report_{DateTime.Now:yyyyMMdd}.pdf");
    }
    
    public static byte[] GenerateAccountingReport(AccountingDashboard dashboard)
    {
        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Margin(20);
                
                // Header
                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Accounting Dashboard Report").Bold().FontSize(24);
                        col.Item().Text($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(12);
                    });
                });

                page.Content().Column(col =>
                {
                    // Financial Overview Section
                    col.Item().PaddingTop(20).Text("Financial Overview").Bold().FontSize(20);
                    col.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        // Assets & Liabilities
                        table.Cell().Background("#EEF2FF").Padding(10).Column(cell =>
                        {
                            cell.Item().Text("Total Assets").Bold();
                            cell.Item().PaddingTop(5).Text($"{dashboard.TotalAssets:C}");
                        });

                        table.Cell().Background("#FEF2F2").Padding(10).Column(cell =>
                        {
                            cell.Item().Text("Total Liabilities").Bold();
                            cell.Item().PaddingTop(5).Text($"{dashboard.TotalLiabilities:C}");
                        });

                        table.Cell().Background("#ECFDF5").Padding(10).Column(cell =>
                        {
                            cell.Item().Text("Total Equity").Bold();
                            cell.Item().PaddingTop(5).Text($"{dashboard.TotalEquity:C}");
                        });
                    });

                    // Profit & Loss Overview
                    col.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Cell().Background("#FEF3C7").Padding(10).Column(cell =>
                        {
                            cell.Item().Text("Total Revenue").Bold();
                            cell.Item().PaddingTop(5).Text($"{dashboard.TotalRevenue:C}");
                        });

                        table.Cell().Background("#FEE2E2").Padding(10).Column(cell =>
                        {
                            cell.Item().Text("Total Expenses").Bold();
                            cell.Item().PaddingTop(5).Text($"{dashboard.TotalExpenses:C}");
                        });

                        table.Cell().Background("#DCF7E3").Padding(10).Column(cell =>
                        {
                            cell.Item().Text("Net Income").Bold();
                            cell.Item().PaddingTop(5).Text($"{dashboard.NetIncome:C}");
                        });
                    });

                    // Monthly Analysis Section
                    col.Item().PaddingTop(30).Text("Monthly Financial Analysis").Bold().FontSize(20);
                    col.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Background("#D1D5DB").Padding(5).Text("Month").Bold();
                            header.Cell().Background("#D1D5DB").Padding(5).Text("Revenue").Bold();
                            header.Cell().Background("#D1D5DB").Padding(5).Text("Expenses").Bold();
                        });

                        // Content
                        foreach (var month in dashboard.MonthlyRevenue.Keys.OrderByDescending(x => x))
                        {
                            table.Cell().Padding(5).Text(month);
                            table.Cell().Padding(5).Text($"{dashboard.MonthlyRevenue[month]:C}");
                            table.Cell().Padding(5).Text($"{dashboard.MonthlyExpenses[month]:C}");
                        }
                    });

                    // Account Distribution Section
                    col.Item().PaddingTop(30).Text("Account Type Distribution").Bold().FontSize(20);
                    col.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Background("#D1D5DB").Padding(5).Text("Account Type").Bold();
                            header.Cell().Background("#D1D5DB").Padding(5).Text("Balance").Bold();
                        });

                        // Content
                        foreach (var account in dashboard.AccountTypeDistribution)
                        {
                            table.Cell().Padding(5).Text(account.Key);
                            table.Cell().Padding(5).Text($"{account.Value:C}");
                        }
                    });

                    // Recent Transactions Section
                    if (dashboard.RecentTransactions.Any())
                    {
                        col.Item().PaddingTop(30).Text("Recent Transactions").Bold().FontSize(20);
                        col.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Background("#D1D5DB").Padding(5).Text("Date").Bold();
                                header.Cell().Background("#D1D5DB").Padding(5).Text("Description").Bold();
                                header.Cell().Background("#D1D5DB").Padding(5).Text("Status").Bold();
                            });

                            // Content - Show only last 5 transactions
                            foreach (var transaction in dashboard.RecentTransactions.Take(5))
                            {
                                table.Cell().Padding(5).Text(transaction.TransactionDate.ToShortDateString());
                                table.Cell().Padding(5).Text(transaction.Description);
                                table.Cell().Padding(5).Text($"{transaction.Status}");
                            }
                        });
                    }

                    // Summary Section
                    col.Item().PaddingTop(30).Background("#F3F4F6").Padding(10).Column(summaryCol =>
                    {
                        summaryCol.Item().Text("Financial Summary").Bold().FontSize(16);
                        summaryCol.Item().PaddingTop(10).Text(text =>
                        {
                            var currentRatio = dashboard.TotalLiabilities != 0 
                                ? dashboard.TotalAssets / dashboard.TotalLiabilities 
                                : 0;
                            var profitMargin = dashboard.TotalRevenue != 0 
                                ? (dashboard.NetIncome / dashboard.TotalRevenue) * 100 
                                : 0;

                            text.Line($"Current Ratio: {currentRatio:F2}");
                            text.Line($"Profit Margin: {profitMargin:F2}%");
                            text.Line($"Asset to Equity Ratio: {(dashboard.TotalEquity != 0 ? dashboard.TotalAssets / dashboard.TotalEquity : 0):F2}");
                        });
                    });
                });

                // Footer
                page.Footer().AlignRight().Text(text =>
                {
                    text.Span("Page ").FontSize(10);
                    text.CurrentPageNumber().FontSize(10);
                    text.Span(" of ").FontSize(10);
                    text.TotalPages().FontSize(10);
                });
            });
        }).GeneratePdf();
    }
    
}