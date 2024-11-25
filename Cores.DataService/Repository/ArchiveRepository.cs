using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class ArchiveRepository : Repository<Archive>,  IArchiveRepository
{
    private readonly ApplicationDbContext _db;

    public ArchiveRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Archive archive)
    {
        var archiveFromDb = await _db.Archives.FirstOrDefaultAsync(a => a.Id == archive.Id);
        if (archiveFromDb is null)  
            return;
        archiveFromDb.EmployeeId = archive.EmployeeId;
        archiveFromDb.ArchiveTypeId = archive.ArchiveTypeId;
        archiveFromDb.Path = archive.Path;
        archiveFromDb.Description = archive.Description;
        archiveFromDb.UploadDate = archive.UploadDate;
        archiveFromDb.ExpiryDate = archive.ExpiryDate;
    }
}