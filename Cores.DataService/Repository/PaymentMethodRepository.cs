using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;

namespace Cores.DataService.Repository;

public class PaymentMethodRepository : Repository<PaymentMethod>, IPaymentMethodRepository
{

    private readonly ApplicationDbContext _db;
    public PaymentMethodRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(PaymentMethod paymentMethod)
    {
        _db.PaymentMethods.Update(paymentMethod);
    }
}