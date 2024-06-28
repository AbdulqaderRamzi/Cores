using Cores.Models.CRM;

namespace Cores.DataService.Repository.IRepository;

public interface IPurchaseRepository : IRepository<Purchase>
{
    void Update(Purchase purchase);
}