using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class HolidayTypeRepository : Repository<HolidayType>, IHolidayTypeRepository
{
    private readonly ApplicationDbContext _db;
    
    public HolidayTypeRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(HolidayType holidayType)
    {
        var holidayTypeFromDb = await _db.HolidayTypes.FirstOrDefaultAsync(ht => ht.Id == holidayType.Id);
        if (holidayTypeFromDb is null)
            return;
            
        holidayTypeFromDb.Name = holidayType.Name;
        holidayTypeFromDb.Description = holidayType.Description;
        holidayTypeFromDb.ColorCode = holidayType.ColorCode;
        holidayTypeFromDb.IsActive = holidayType.IsActive;
    }
}