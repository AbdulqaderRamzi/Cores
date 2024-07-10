using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IDepartmentRepository : IRepository<Department>
{
    Task Update(Department department);
}