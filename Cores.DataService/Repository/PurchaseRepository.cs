using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.CRM;

namespace Cores.DataService.Repository;

public class PurchaseRepository : Repository<Purchase>, IPurchaseRepository
{
    private readonly ApplicationDbContext _db;
    
    public PurchaseRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Purchase purchase)
    {
        _db.Purchases.Update(purchase);
    }
}