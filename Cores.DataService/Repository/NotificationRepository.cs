using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models;

namespace Cores.DataService.Repository;

public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    private readonly ApplicationDbContext _db;
    public NotificationRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Notification notification)
    {
        _db.Notifications.Update(notification);
    }
}