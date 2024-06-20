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
    }

    public async Task SaveAsync() => await _db.SaveChangesAsync();
}