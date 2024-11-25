using Cores.Models.CRM;

namespace Cores.DataService.Repository.IRepository;

public interface IProblemRepository : IRepository<Problem>
{
    Task Update(Problem problem);
    Task<(int OpenProblems, int PendingProblems, int ClosedProblems)> GetStatusCounts();
}