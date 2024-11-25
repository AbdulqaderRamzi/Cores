using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IPerformanceReviewRepository : IRepository<PerformanceReview>
{
    Task Update(PerformanceReview performanceReview);
}