using Cores.Models.Accounting;

namespace Cores.DataService.Repository.IRepository;

public interface IJournalRepository : IRepository<Journal>
{
    Task Update(Journal journal);
}