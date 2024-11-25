using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IHolidayTypeRepository : IRepository<HolidayType>
{
    Task Update(HolidayType holidayType);
}