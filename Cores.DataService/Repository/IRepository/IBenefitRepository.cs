using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IBenefitRepository : IRepository<Benefit>
{
    Task Update(Benefit benefit);
}