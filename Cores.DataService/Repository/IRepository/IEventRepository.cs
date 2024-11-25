using Cores.Models.CRM;

namespace Cores.DataService.Repository.IRepository;

public interface IEventRepository : IRepository<Event>
{
    Task Update(Event @event);
    Task<int> PendingEventsCount();
}