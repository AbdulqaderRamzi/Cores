using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IWorkScheduleRepository : IRepository<WorkSchedule>
{
    Task Update(WorkSchedule workSchedule);
}