using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.CRM;

namespace Cores.DataService.Repository;

public class OrderRepository : Repository<Order>, IOrderRepository
{

    private readonly ApplicationDbContext _db;

    public OrderRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Order order)
    {
        _db.Update(order);
    }
}