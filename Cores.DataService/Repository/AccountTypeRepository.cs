using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class AccountTypeRepository : Repository<AccountType>, IAccountTypeRepository
{
    private readonly ApplicationDbContext _db;

    public AccountTypeRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(AccountType accountType)
    {
        var accountTypeFromDb = await _db.AccountTypes.FirstOrDefaultAsync(at => at.Id == accountType.Id);
        if (accountTypeFromDb is null)
            return;
        accountTypeFromDb.Type = accountType.Type;
        accountTypeFromDb.Description = accountType.Description;
    }
}