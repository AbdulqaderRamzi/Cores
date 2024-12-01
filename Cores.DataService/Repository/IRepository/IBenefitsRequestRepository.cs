using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IBenefitsRequestRepository : IRepository<BenefitsRequest>
{
    Task Update(BenefitsRequest benefitsRequest);    
}