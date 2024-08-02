using Cores.Models.Accounting;

namespace Cores.DataService.Repository.IRepository;

public interface IJournalEntryRepository : IRepository<JournalEntry>
{
    Task Update(JournalEntry journalEntry);
}