using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IJobApplicationRepository : IRepository<JobApplication>
{
    Task Update(JobApplication jobApplication);
}