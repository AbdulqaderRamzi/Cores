using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class PerformanceReviewRepository : Repository<PerformanceReview>, IPerformanceReviewRepository
{
    private readonly ApplicationDbContext _db;

    public PerformanceReviewRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(PerformanceReview performanceReview)
    {
        var performanceReviewDb = await _db.PerformanceReviews.FirstOrDefaultAsync(pr => pr.Id == performanceReview.Id);
        if (performanceReviewDb is null)
            return;
        performanceReviewDb.EmployeeId = performanceReview.EmployeeId;
        performanceReviewDb.ReviewerId = performanceReview.ReviewerId;
        performanceReviewDb.PerformanceScore = performanceReview.PerformanceScore;
        performanceReviewDb.Comments = performanceReview.Comments;
    }
}