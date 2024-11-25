using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IEmployeeLeaveBalanceRepository : IRepository<EmployeeLeaveBalance>
{
    Task Update(EmployeeLeaveBalance employeeLeaveBalance);
}