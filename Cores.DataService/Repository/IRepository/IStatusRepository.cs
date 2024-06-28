using Cores.Models.Accounting;

namespace Cores.DataService.Repository.IRepository;

public interface IStatusRepository : IRepository<Status>
{
    void Update(Status status);
}