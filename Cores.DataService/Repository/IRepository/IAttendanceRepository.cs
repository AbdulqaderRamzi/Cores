using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IAttendanceRepository : IRepository<Attendance>
{
    Task Update(Attendance attendance);
    Task<IDictionary<string, int>> GetMonthlyLateEmployees();
}