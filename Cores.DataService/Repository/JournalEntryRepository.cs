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
        journalEntryFromDb.AccountId = journalEntry.AccountId;
        journalEntryFromDb.Debit = journalEntry.Debit;
        journalEntryFromDb.Credit = journalEntry.Credit;
        journalEntryFromDb.Amount = journalEntry.Amount;
        journalEntryFromDb.UpdatedAt = DateTime.Now;
        
    }
}