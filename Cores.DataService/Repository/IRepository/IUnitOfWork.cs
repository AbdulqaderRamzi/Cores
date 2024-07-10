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
    ICurrencyRepository Currency { get; }
    public IProductRepository Product { get; }
    public IOrderRepository Order { get; }
    public IProblemRepository Problem { get; }
    public IProblemTypeRepository ProblemType { get; }
    public IEventRepository Event { get; }
    public IEventTypeRepository EventType { get; }
    public ITodoRepository Todo { get; }
    public INotificationRepository Notification { get; }
    public IDepartmentRepository Department { get; }
    public IPositionRepository Position { get; }

    Task SaveAsync();
}