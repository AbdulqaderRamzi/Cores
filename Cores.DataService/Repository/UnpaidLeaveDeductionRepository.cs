using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class UnpaidLeaveDeductionRepository : Repository<UnpaidLeaveDeduction>, IUnpaidLeaveDeductionRepository
{
    private readonly ApplicationDbContext _db;

    public UnpaidLeaveDeductionRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(UnpaidLeaveDeduction unpaidLeaveDeduction)
    {
        var unpaidLeaveDeductionFromDb = await _db.UnpaidLeaveDeductions
            .FirstOrDefaultAsync(u => u.Id == unpaidLeaveDeduction.Id);
        if (unpaidLeaveDeductionFromDb is null)
        {
            return;
        }
        unpaidLeaveDeductionFromDb.Deduction = unpaidLeaveDeduction.Deduction;
        unpaidLeaveDeductionFromDb.DateTime = unpaidLeaveDeduction.DateTime;
        unpaidLeaveDeductionFromDb.ApplicationUserId = unpaidLeaveDeduction.ApplicationUserId;
    }
}