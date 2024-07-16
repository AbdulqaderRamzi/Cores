using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface ISalaryRepository : IRepository<Salary>
{
    Task Update(Salary salary);
}