using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    private readonly ApplicationDbContext _db;

    public TransactionRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Transaction transaction)
    {
        var transactionFromDb = await _db.Transactions.FirstOrDefaultAsync(t => t.Id == transaction.Id);
        if (transactionFromDb is null)
            return;
        transactionFromDb.UpdatedAt = DateTime.Now;
        transactionFromDb.Amount = transaction.Amount;
        transactionFromDb.DebitAccountId = transaction.DebitAccountId;
        transactionFromDb.CreditAccountId = transaction.CreditAccountId;
    }
}