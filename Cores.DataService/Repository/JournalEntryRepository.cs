using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class JournalEntryRepository : Repository<JournalEntry>, IJournalEntryRepository
{
    private readonly ApplicationDbContext _db;

    public JournalEntryRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(JournalEntry journalEntry)
    {
        var journalEntryFromDb = await _db.JournalEntries.FirstOrDefaultAsync(je => je.Id == journalEntry.Id);
        if (journalEntryFromDb is null)
            return;
        journalEntryFromDb.JournalId = journalEntry.JournalId;
        journalEntryFromDb.EntryNumber = journalEntry.EntryNumber;
        journalEntryFromDb.EntryDate = journalEntry.EntryDate;
        journalEntryFromDb.Description = journalEntry.Description;
        journalEntryFromDb.IsPosted = journalEntry.IsPosted;
        journalEntryFromDb.PostedDate = journalEntry.PostedDate;
        journalEntryFromDb.PostedBy = journalEntry.PostedBy;
        journalEntryFromDb.CreatedAt = journalEntry.CreatedAt;
        journalEntryFromDb.CreatedBy = journalEntry.CreatedBy;
        
    }
}