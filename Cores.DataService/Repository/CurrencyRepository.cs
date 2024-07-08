using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class CurrencyRepository : Repository<Currency>, ICurrencyRepository
{
    private readonly ApplicationDbContext _db;
    public CurrencyRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Currency currency)
    {
        var currencyFromDb = await _db.Currencies.FirstOrDefaultAsync(c => c.Id == currency.Id);
        if (currencyFromDb is null)
            return;
        currencyFromDb.Name = currency.Name;
        currencyFromDb.Code = currency.Code;
    }
}