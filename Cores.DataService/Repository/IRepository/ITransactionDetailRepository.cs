using Cores.Models.Accounting;

namespace Cores.DataService.Repository.IRepository;

public interface ITransactionDetailRepository : IRepository<TransactionDetail>
{
    Task Update(TransactionDetail transactionDetail);
}