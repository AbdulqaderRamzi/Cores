using Cores.Models.Accounting;

namespace Cores.DataService.Repository.IRepository;

public interface IJournalTypeRepository : IRepository<JournalType>
{
    Task Update(JournalType journalType);
}