using Cores.Models.Accounting;

namespace Cores.DataService.Repository.IRepository;

public interface IProductRepository : IRepository<Product>
{
    Task Update(Product product);
}