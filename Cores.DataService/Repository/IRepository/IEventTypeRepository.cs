using Cores.Models.CRM;

namespace Cores.DataService.Repository.IRepository;

public interface IEventTypeRepository : IRepository<EventType>
{
    Task Update(EventType eventType);
}