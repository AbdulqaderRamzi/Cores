using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.CRM;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class EventRepository : Repository<Event>, IEventRepository
{
    private readonly ApplicationDbContext _db;
    
    public EventRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Event @event)
    {
        var eventFromDb = await _db.Events.FirstOrDefaultAsync(e => e.Id == @event.Id);
        if (eventFromDb is null)
            return;
        eventFromDb.ContactId = @event.ContactId;
        eventFromDb.EventTypeId = @event.EventTypeId;
        eventFromDb.Status = @event.Status;
        eventFromDb.Description = @event.Description;
        eventFromDb.ModifiedById = @event.ModifiedById;
        eventFromDb.DateTime = @event.DateTime;
    }

    public async Task<int> PendingEventsCount()
    {
        return await _db.Events.CountAsync(e => e.Status == "pending");
        
    }
}