using Cores.Models.Accounting;

namespace Cores.DataService.Repository.IRepository;

public interface IPaymentMethodRepository : IRepository<PaymentMethod>
{
    void Update(PaymentMethod paymentMethod);
}