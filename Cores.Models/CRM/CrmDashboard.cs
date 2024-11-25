namespace Cores.Models.CRM;

public class CrmDashboard
{
    public int CustomersCount { get; set; }
    public int LeadsCount { get; set; }
    public int PendingEvents { get; set; }
    public int OpenProblems { get; set; }
    public int PendingProblems { get; set; }
    public int ClosedProblems { get; set; }
    public int MonthlyPurchase { get; set; }
    public IDictionary<string, decimal> MonthlyEarnings { get; set; }
    public IDictionary<string, int> MonthlyContacts { get; set; }
}