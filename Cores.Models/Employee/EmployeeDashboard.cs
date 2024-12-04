namespace Cores.Models.Employee;

public class EmployeeDashboard
{
    public string EmployeeId { get; set; }
    public decimal CurrentSalary { get; set; }
    public int PendingRequests { get; set; }
    
    public decimal? TotalBenefits { get; set; }
    public decimal? TotalDeductions { get; set; }

    public double LeaveBalance { get; set; }
    public Dictionary<string, decimal?> MonthlyEarnings { get; set; }
    public IDictionary<string, int> MonthlyRequests { get; set; }
    public int RejectedRequests { get; set; }
    public int ApprovedRequests { get; set; }
}