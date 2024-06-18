namespace Cores.DataService.Repository.IRepository;

public interface IUnitOfWork
{
    IApplicationUserRepository ApplicationUser{ get; }
    ICheckBoxRepository CheckBox { get; }
    IActivityLogRepository ActivityLog { get; }
    ITagRepository Tag { get; }
    IMessagePayloadRepository MessagePayload { get; }
    ICustomerRepository Customer { get; }
    Task SaveAsync();
}