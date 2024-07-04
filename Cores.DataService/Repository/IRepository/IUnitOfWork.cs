namespace Cores.DataService.Repository.IRepository;

public interface IUnitOfWork
{
    IApplicationUserRepository ApplicationUser{ get; }
    ICheckBoxRepository CheckBox { get; }
    IActivityLogRepository ActivityLog { get; }
    ITagRepository Tag { get; }
    IMessagePayloadRepository MessagePayload { get; }
    IContactRepository Contact { get; }
    ILanguageRepository Language { get; }
    IPurchaseRepository Purchase { get; }
    IPaymentMethodRepository PaymentMethod { get; }
    IStatusRepository Status { get; }
    ICurrencyRepository Currency { get; }
    public IProductRepository Product { get; }
    public IOrderRepository Order { get; }
    public IProblemRepository Problem { get; }
    public IProblemTypeRepository ProblemType { get; }
    public IEventRepository Event { get; }
    public IEventTypeRepository EventType { get; }

    Task SaveAsync();
}