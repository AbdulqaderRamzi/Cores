using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IArchiveRepository : IRepository<Archive>
{
    Task Update(Archive archive);
}