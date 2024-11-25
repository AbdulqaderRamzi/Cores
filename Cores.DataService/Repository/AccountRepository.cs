using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class AccountRepository : Repository<Account>, IAccountRepository
{
    private readonly ApplicationDbContext _db;

    public AccountRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Account account)
    { 
        var accountFromDb = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == account.Id);
        if (accountFromDb is null) 
            return;
        accountFromDb.Name = account.Name;
        accountFromDb.Type = account.Type;
        accountFromDb.Category = account.Category;
        accountFromDb.Balance = account.Balance;
        accountFromDb.IsActive = account.IsActive;
        accountFromDb.CreatedAt = account.CreatedAt;
        accountFromDb.CreatedBy = account.CreatedBy;
    }
}