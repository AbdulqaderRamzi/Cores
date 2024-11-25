using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class TransactionDetailRepository : Repository<TransactionDetail>, ITransactionDetailRepository
{
    private readonly ApplicationDbContext _db;

    public TransactionDetailRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(TransactionDetail transactionDetail)
    {
        var detailFromDb = await _db.TransactionDetails
            .FirstOrDefaultAsync(td => td.Id == transactionDetail.Id);
        if (detailFromDb is null)
        {
            return;
        }
        detailFromDb.AccountId = transactionDetail.AccountId;
        detailFromDb.DebitAmount = transactionDetail.DebitAmount;
        detailFromDb.CreditAmount = transactionDetail.CreditAmount;
        detailFromDb.Description = transactionDetail.Description;
    }
}