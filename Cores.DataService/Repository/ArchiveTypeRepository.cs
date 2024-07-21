using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class ArchiveTypeRepository : Repository<ArchiveType>, IArchiveTypeRepository
{
    private readonly ApplicationDbContext _db;

    public ArchiveTypeRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(ArchiveType archiveType)
    {
        var archiveTypeFromDb = await _db.ArchiveTypes.FirstOrDefaultAsync(at => at.Id == archiveType.Id);
        if (archiveTypeFromDb is null)
            return;
        archiveTypeFromDb.Name = archiveType.Name;
        archiveTypeFromDb.Description = archiveType.Description;
    }
}