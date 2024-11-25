using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class TaxRepository : Repository<Tax>, ITaxRepository
{
    private readonly ApplicationDbContext _db;

    public TaxRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Tax tax)
    {
        var taxFromDb = await _db.Taxes.FirstOrDefaultAsync(t => t.Id == tax.Id);
        if (taxFromDb is null)
            return;
        taxFromDb.Name = tax.Name;
        taxFromDb.Status = tax.Status;
        taxFromDb.Rate = tax.Rate;
    }
}