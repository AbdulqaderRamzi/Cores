using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IDeductionRepository : IRepository<Deduction>
{
    Task Update(Deduction deduction);
}