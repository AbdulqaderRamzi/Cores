using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IEmployeeTrainingRepository : IRepository<EmployeeTraining>
{
    Task Update(EmployeeTraining employeeTraining);
}