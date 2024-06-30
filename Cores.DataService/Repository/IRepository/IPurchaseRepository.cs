using Cores.Models.CRM;

namespace Cores.DataService.Repository.IRepository;

public interface IPurchaseRepository : IRepository<Purchase>
{
    Task Update(Purchase purchase);
}