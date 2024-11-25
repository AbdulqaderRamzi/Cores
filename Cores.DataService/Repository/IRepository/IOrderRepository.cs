using Cores.Models.CRM;

namespace Cores.DataService.Repository.IRepository;

public interface IOrderRepository : IRepository<Order>
{
    void Update(Order order);
}