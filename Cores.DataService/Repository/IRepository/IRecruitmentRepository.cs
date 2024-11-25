using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IRecruitmentRepository : IRepository<Recruitment>
{
    Task Update(Recruitment recruitment);
}