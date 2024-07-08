using Cores.Models;

namespace Cores.DataService.Repository.IRepository;

public interface INotificationRepository : IRepository<Notification>
{

    void Update(Notification notification);

}