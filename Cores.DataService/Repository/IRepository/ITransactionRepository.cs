using Cores.Models.Accounting;

namespace Cores.DataService.Repository.IRepository;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task Update(Transaction transaction);
}