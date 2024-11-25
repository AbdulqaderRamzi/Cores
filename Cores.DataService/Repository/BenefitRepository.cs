using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class BenefitRepository : Repository<Benefit>, IBenefitRepository
{
    private readonly ApplicationDbContext _db;
    
    public BenefitRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Benefit benefit)
    {
        var benefitFromDb = await _db.Benefits.FirstOrDefaultAsync(b => b.Id == benefit.Id);
        if (benefitFromDb is null)
            return;
        benefitFromDb.Name = benefit.Name;
        benefitFromDb.Amount = benefit.Amount;
        benefitFromDb.Description = benefit.Description;
        benefitFromDb.EligibilityCriteria = benefit.EligibilityCriteria;
    }
}