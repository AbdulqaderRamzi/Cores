using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class HolidayRepository : Repository<Holiday>, IHolidayRepository
{
    private readonly ApplicationDbContext _db;
    
    public HolidayRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Holiday holiday)
    {
        var holidayFromDb = await _db.Holidays.FirstOrDefaultAsync(h => h.Id == holiday.Id);
        if (holidayFromDb is null)
            return;
            
        holidayFromDb.Name = holiday.Name;
        holidayFromDb.HolidayTypeId = holiday.HolidayTypeId;
        holidayFromDb.Description = holiday.Description;
        holidayFromDb.IsRecurringYearly = holiday.IsRecurringYearly;
        holidayFromDb.IsWorkingDay = holiday.IsWorkingDay;
        holidayFromDb.Location = holiday.Location;
        holidayFromDb.StartAt = holiday.StartAt;
        holidayFromDb.EndAt = holiday.EndAt;
        holidayFromDb.IsActive = holiday.IsActive;
    }

    public async Task<int> ActiveHolidaysCount()
    {
        return await _db.Holidays.CountAsync(h => h.IsActive);
    }
}