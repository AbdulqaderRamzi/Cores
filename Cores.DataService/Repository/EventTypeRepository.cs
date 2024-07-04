using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.CRM;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class EventTypeRepository : Repository<EventType>, IEventTypeRepository
{
    private readonly ApplicationDbContext _db;

    public EventTypeRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(EventType eventType)
    {
        var eventTypeFromDb = await _db.EventTypes.FirstOrDefaultAsync(et => et.Id == eventType.Id);
        if (eventTypeFromDb is null)
            return;
        eventTypeFromDb.Type = eventType.Type;
        eventTypeFromDb.Description = eventType.Description;
    }
}