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
        purchaseFromDb.Status = purchase.Status;
        purchaseFromDb.PurchaseDate = purchase.PurchaseDate;
        purchaseFromDb.InvoiceEndDate = purchase.InvoiceEndDate;
        purchaseFromDb.CurrencyId = purchase.CurrencyId;
        purchaseFromDb.PaymentMethodId = purchase.PaymentMethodId;
        purchaseFromDb.Note = purchase.Note;
        purchaseFromDb.ContactId = purchase.ContactId;
        purchaseFromDb.Contact = purchase.Contact;
        purchaseFromDb.TaxId = purchase.TaxId;
        purchaseFromDb.Orders.Clear();
        foreach (var order in purchase.Orders)
        {
            purchaseFromDb.Orders.Add(order);
        }
    }

    public async Task RemovePurchaseWithOrdersRaw(int? purchaseId)
    {
        await _db.Database.BeginTransactionAsync();
        try
        {
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM Orders WHERE PurchaseId = {purchaseId}; DELETE FROM Purchases WHERE Id = {purchaseId}"
            );
            await _db.Database.CommitTransactionAsync();
        }
        catch
        {
            await _db.Database.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<IDictionary<string, decimal>> GetMonthlyEarnings()
    {
        var oneYearAgo = DateTime.Now.AddYears(-1);
        var earnings = await _db.Purchases
            .Where(p => p.PurchaseDate >= oneYearAgo)
            .GroupBy(p => new { p.PurchaseDate.Year, p.PurchaseDate.Month })
            .Select(g => new 
            {
                YearMonth = new DateTime(g.Key.Year, g.Key.Month, 1),
                Total = g.Sum(p => p.PurchaseAmount)
            })
            .ToDictionaryAsync(
                k => k.YearMonth.ToString("MMM"),
                v => v.Total
            );

        var allMonths = new[] 
        { 
            "Jan", "Feb", "Mar", "Apr", "May", "Jun", 
            "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        };

        return allMonths.ToDictionary(
            month => month,
            month => earnings.TryGetValue(month, out var value) ? value : 0m
        );
    }

    public async Task<int> GetMonthlyPurchase()
    {
        var oneMonthAgo = DateTime.Now.AddMonths(-1);
        return await _db.Purchases
            .Where(p => p.PurchaseDate >= oneMonthAgo)
            .CountAsync();
    }
}