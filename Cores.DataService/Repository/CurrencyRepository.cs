using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;

namespace Cores.DataService.Repository;

public class CurrencyRepository : Repository<Currency>, ICurrencyRepository
{
    private readonly ApplicationDbContext _db;
    public CurrencyRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Currency currency)
    {
        _db.Currencies.Update(currency);
    }
}