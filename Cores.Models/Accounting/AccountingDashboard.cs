using System.ComponentModel.DataAnnotations;

namespace Cores.Models.Accounting;

public class AccountingDashboard
{
    public decimal TotalAssets { get; set; }
    
    public decimal TotalLiabilities { get; set; }
    
    public decimal TotalRevenue { get; set; }
    
    public decimal TotalExpenses { get; set; }
    
    public decimal TotalEquity  { get; set; }
    
    public decimal NetIncome { get; set; }
    
    public IEnumerable<Transaction> RecentTransactions { get; set; }
    public Dictionary<string, decimal> MonthlyRevenue { get; set; }
    public Dictionary<string, decimal> MonthlyExpenses { get; set; }
    public Dictionary<string, decimal> AccountTypeDistribution { get; set; }
}