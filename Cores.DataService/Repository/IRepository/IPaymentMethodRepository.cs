using Cores.Models.Accounting;

namespace Cores.DataService.Repository.IRepository;

public interface IPaymentMethodRepository : IRepository<PaymentMethod>
{
    Task Update(PaymentMethod paymentMethod);
}