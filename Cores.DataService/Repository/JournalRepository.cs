using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class JournalRepository : Repository<Journal>, IJournalRepository
{
    private readonly ApplicationDbContext _db;

    public JournalRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Journal journal)
    {
        var journalFromDb = await _db.Journals.FirstOrDefaultAsync(j => j.Id == journal.Id);
        if (journalFromDb is null) 
            return;
        journalFromDb.JournalTypeId = journal.JournalTypeId;
        journalFromDb.Description = journal.Description;
    }
}