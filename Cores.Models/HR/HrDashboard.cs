namespace Cores.Models.HR;

public class HrDashboard
{
    public int EmployeesCount { get; set; }
    public int CrmCount { get; set; }
    public int AccountingCount { get; set; }
    public int HrCount { get; set; }
    public int CompletedTodoCount { get; set; }
    public int InProgressTodoCount { get; set; }
    public int FailedTodoCount { get; set; }
    public IDictionary<string, int> LateEmployees { get; set; }
    public IDictionary<string, int> JobApplications { get; set; }
}