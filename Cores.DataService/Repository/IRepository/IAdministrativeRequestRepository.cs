using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IAdministrativeRequestRepository : IRepository<AdministrativeRequest>
{
    Task Update(AdministrativeRequest administrativeRequest);
}