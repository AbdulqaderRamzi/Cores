using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;

namespace Cores.DataService.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;
    public IApplicationUserRepository ApplicationUser { get; }
    public ICheckBoxRepository CheckBox { get; }
    public IActivityLogRepository ActivityLog { get; }
    public ITagRepository Tag { get; }
    public IMessagePayloadRepository MessagePayload { get; }
    public ICustomerRepository Customer { get; }
    public ILanguageRepository Language { get; }
    public IPurchaseRepository Purchase { get; }
    public IPaymentMethodRepository PaymentMethod { get; }
    public IStatusRepository Status { get; }
    public ICurrencyRepository Currency { get; }
    public IProductRepository Product { get; }
    public IOrderRepository Order { get; }

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        ApplicationUser = new ApplicationUserRepository(_db);
        CheckBox = new CheckBoxRepository(_db);
        ActivityLog = new ActivityLogRepository(_db);
        Tag = new TagRepository(_db);
        MessagePayload = new MessagePayloadRepository(_db);
        Customer = new CustomerRepository(_db);
        Language = new LanguageRepository(_db);
        Purchase = new PurchaseRepository(_db);
        PaymentMethod = new PaymentMethodRepository(_db);
        Status = new StatusRepository(_db);
        Currency = new CurrencyRepository(_db);
        Product = new ProductRepository(_db);
        Order = new OrderRepository(_db);
    }

    public async Task SaveAsync() => await _db.SaveChangesAsync();
}