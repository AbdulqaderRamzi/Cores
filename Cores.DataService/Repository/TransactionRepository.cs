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
        var transactionFromDb = await _db.Transactions.Include(t => t.Details).FirstOrDefaultAsync(t => t.Id == transaction.Id);
        if (transactionFromDb is null)
            return;
        transactionFromDb.ReferenceNo = transaction.ReferenceNo;
        transactionFromDb.TransactionDate = transaction.TransactionDate;
        transactionFromDb.Description = transaction.Description;
        transactionFromDb.TotalDebit = transaction.TotalDebit;
        transactionFromDb.TotalCredit = transaction.TotalCredit;
        transactionFromDb.Status = transaction.Status;
        transactionFromDb.CreatedAt = transaction.CreatedAt;
        transactionFromDb.CreatedBy = transaction.CreatedBy;
        transactionFromDb.Details.Clear();
        transactionFromDb.Details.AddRange(transaction.Details);
    }
}