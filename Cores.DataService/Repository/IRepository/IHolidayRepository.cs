using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IHolidayRepository : IRepository<Holiday>
{
    Task Update(Holiday holiday);
    Task<int> ActiveHolidaysCount();
}