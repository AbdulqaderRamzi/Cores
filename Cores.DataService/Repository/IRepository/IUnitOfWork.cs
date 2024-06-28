namespace Cores.DataService.Repository.IRepository;

public interface IUnitOfWork
{
    IApplicationUserRepository ApplicationUser{ get; }
    ICheckBoxRepository CheckBox { get; }
    IActivityLogRepository ActivityLog { get; }
    ITagRepository Tag { get; }
    IMessagePayloadRepository MessagePayload { get; }
    ICustomerRepository Customer { get; }
    ILanguageRepository Language { get; }
    IPurchaseRepository Purchase { get; }
    IPaymentMethodRepository PaymentMethod { get; }
    IStatusRepository Status { get; }
    ICurrencyRepository Currency { get; }
    public IProductRepository Product { get; }
    public IOrderRepository Order { get; }

    Task SaveAsync();
}