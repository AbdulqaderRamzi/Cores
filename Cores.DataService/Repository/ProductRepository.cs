using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _db;
    
    public ProductRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Product product)
    {
        var productFromDb = await _db.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
        if (productFromDb is null)
            return;
        productFromDb.Name = product.Name;
        productFromDb.UnitPrice = product.UnitPrice;
    }
}