using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IEmployeeDeductionRepository : IRepository<EmployeeDeduction>
{
    Task Update(EmployeeDeduction employeeDeduction);
}