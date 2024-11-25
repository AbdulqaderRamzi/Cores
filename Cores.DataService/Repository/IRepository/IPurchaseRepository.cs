using Cores.Models.CRM;

namespace Cores.DataService.Repository.IRepository;

public interface IPurchaseRepository : IRepository<Purchase>
{
    Task Update(Purchase purchase);
    Task RemovePurchaseWithOrdersRaw(int? purchaseId);
    Task<IDictionary<string, decimal>> GetMonthlyEarnings();
    Task<int> GetMonthlyPurchase();
}