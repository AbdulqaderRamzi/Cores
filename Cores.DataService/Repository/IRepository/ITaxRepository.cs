using Cores.Models.Accounting;

namespace Cores.DataService.Repository.IRepository;

public interface ITaxRepository : IRepository<Tax>
{
    Task Update(Tax tax);
}