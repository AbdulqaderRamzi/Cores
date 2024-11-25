using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class PaymentMethodRepository : Repository<PaymentMethod>, IPaymentMethodRepository
{

    private readonly ApplicationDbContext _db;
    public PaymentMethodRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(PaymentMethod paymentMethod)
    {
        var paymentMethodFromDb = await _db.PaymentMethods.FirstOrDefaultAsync(pm => pm.Id == paymentMethod.Id);
        if (paymentMethodFromDb is null)
            return;
        paymentMethodFromDb.Name = paymentMethod.Name;
    }
}