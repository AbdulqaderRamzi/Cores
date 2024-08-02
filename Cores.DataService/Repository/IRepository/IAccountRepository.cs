using Cores.Models.Accounting;

namespace Cores.DataService.Repository.IRepository;

public interface IAccountRepository : IRepository<Account>
{
    Task Update(Account account);
}