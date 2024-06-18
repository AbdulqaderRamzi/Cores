
using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models;

namespace Cores.DataService.Repository;

public class ActivityLogRepository : Repository<ActivityLog>, IActivityLogRepository
{
    private ApplicationDbContext _db;
    public ActivityLogRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }
}