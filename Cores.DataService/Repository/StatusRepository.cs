using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;

namespace Cores.DataService.Repository;

public class StatusRepository : Repository<Status>, IStatusRepository
{
    private readonly ApplicationDbContext _db;
    
    public StatusRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;;
    }

    public void Update(Status status)
    {
        _db.Statuses.Update(status);
    }
}