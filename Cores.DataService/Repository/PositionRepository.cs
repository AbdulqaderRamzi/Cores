using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class PositionRepository : Repository<Position>, IPositionRepository
{
    private readonly ApplicationDbContext _db;

    public PositionRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Position position)
    {
        var positionFromDb = await _db.Positions.FirstOrDefaultAsync(p => p.Id == position.Id);
        if (positionFromDb is null)
            return;
        positionFromDb.Title = position.Title;
        positionFromDb.DepartmentId = position.DepartmentId;
        positionFromDb.JobDescription = position.JobDescription;
        positionFromDb.SalaryRange = position.SalaryRange;
    }
}