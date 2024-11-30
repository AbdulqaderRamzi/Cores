using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Cores.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cores.Web.Areas.Accounting.Controllers;

[Area("Accounting")]
[Authorize(Roles = SD.ACCOUNTING_ROLE)]
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
    
/*
 *
 
 // Calculate net income (Revenue - Expenses) for each month
    return allMonths.ToDictionary(
        month => month,
        month => {
            var revenue = revenueTransactions.TryGetValue(month, out var rev) ? rev : 0m;
            var expense = expenseTransactions.TryGetValue(month, out var exp) ? exp : 0m;
            return revenue - expense;
        }
    );

 * 
 */            

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
}