using Cores.Models.CRM;

namespace Cores.DataService.Repository.IRepository;

public interface IProblemTypeRepository : IRepository<ProblemType>
{
    Task Update(ProblemType problemType);
}