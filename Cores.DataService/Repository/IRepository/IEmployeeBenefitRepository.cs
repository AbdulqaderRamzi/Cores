using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IEmployeeBenefitRepository : IRepository<EmployeeBenefit>
{
    Task Update(EmployeeBenefit employeeBenefit);
}