using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;

namespace Cores.DataService.Repository;

public class GeneralLedgerRepository : Repository<GeneralLedger>, IGeneralLedgerRepository
{
    private readonly ApplicationDbContext _db;

    public GeneralLedgerRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }
}