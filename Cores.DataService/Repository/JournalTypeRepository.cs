using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class JournalTypeRepository : Repository<JournalType>, IJournalTypeRepository
{
    private readonly ApplicationDbContext _db;
    
    public JournalTypeRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(JournalType journalType)
    {
        var journalTypeFromDb = await _db.JournalTypes.FirstOrDefaultAsync(jt => jt.Id == journalType.Id);
        if (journalTypeFromDb is null)
            return;
        journalTypeFromDb.Type = journalType.Type;
        journalTypeFromDb.Description = journalType.Description;
    }
}