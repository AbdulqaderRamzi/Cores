using Cores.Models.Accounting;

namespace Cores.DataService.Repository.IRepository;

public interface IAccountTypeRepository : IRepository<AccountType>
{
    Task Update(AccountType accountType);
}