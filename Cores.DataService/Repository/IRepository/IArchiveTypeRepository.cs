using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IArchiveTypeRepository : IRepository<ArchiveType>
{
    Task Update(ArchiveType archiveType);
}