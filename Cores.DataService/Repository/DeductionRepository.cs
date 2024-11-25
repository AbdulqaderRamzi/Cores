using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class DeductionRepository : Repository<Deduction>, IDeductionRepository
{
    private readonly ApplicationDbContext _db;
    
    public DeductionRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Deduction deduction)
    {
        var benefitFromDb = await _db.Deductions.FirstOrDefaultAsync(b => b.Id == deduction.Id);
        if (benefitFromDb is null)
            return;
        benefitFromDb.Name = deduction.Name;
        benefitFromDb.Amount = deduction.Amount;
        benefitFromDb.Description = deduction.Description;
        benefitFromDb.EligibilityCriteria = deduction.EligibilityCriteria;
    }
}