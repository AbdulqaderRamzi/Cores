using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.CRM;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class ProblemTypeRepository : Repository<ProblemType>, IProblemTypeRepository
{
    private readonly ApplicationDbContext _db;

    public ProblemTypeRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(ProblemType problemType)
    {
        var problemTypeFromDb = await _db.ProblemTypes.FirstOrDefaultAsync(pt => pt.Id == problemType.Id);
        if (problemTypeFromDb is null)
            return;
        problemTypeFromDb.Type = problemType.Type;
        problemTypeFromDb.Description = problemType.Description;
    }
}