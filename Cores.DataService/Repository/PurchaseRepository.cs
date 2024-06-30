using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.CRM;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class PurchaseRepository : Repository<Purchase>, IPurchaseRepository
{
    private readonly ApplicationDbContext _db;
    
    public PurchaseRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Purchase purchase)
    {
        var purchaseFromDb = await _db.Purchases.Include("Orders").FirstOrDefaultAsync(p => p.Id == purchase.Id);
        if (purchaseFromDb is null)
            return;
        purchaseFromDb.PurchaseAmount = purchase.PurchaseAmount;
        purchaseFromDb.Currency = purchase.Currency;
        purchaseFromDb.Status = purchase.Status;
        purchaseFromDb.PaymentMethod = purchase.PaymentMethod;
        purchaseFromDb.Note = purchase.Note;
        purchaseFromDb.CustomerId = purchase.CustomerId;
        purchaseFromDb.Customer = purchase.Customer;
        purchaseFromDb.Orders.Clear();
        foreach (var order in purchase.Orders)
        {
            purchaseFromDb.Orders.Add(order);
        }
    }
}