using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface ICompensationRequestRepository : IRepository<CompensationRequest>
{
    Task Update(CompensationRequest compensationRequest);
}