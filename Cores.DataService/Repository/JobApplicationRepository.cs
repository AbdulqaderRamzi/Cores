using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class JobApplicationRepository : Repository<JobApplication>, IJobApplicationRepository
{
    private readonly ApplicationDbContext _db;
    
    public JobApplicationRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(JobApplication jobApplication)
    {
        var jobApplicationFromDb = await _db.JobApplications.FirstOrDefaultAsync(ja => ja.Id == jobApplication.Id);
        if (jobApplicationFromDb is null)
            return;
        jobApplicationFromDb.RecruitmentId = jobApplication.RecruitmentId;
        jobApplicationFromDb.Name = jobApplication.Name;
        jobApplicationFromDb.Phone = jobApplication.Phone;
        jobApplicationFromDb.Email = jobApplication.Email;
        jobApplicationFromDb.Resume = jobApplication.Resume;
        jobApplicationFromDb.DateTime = jobApplication.DateTime;
        if (jobApplicationFromDb.Status != "Hired" && jobApplication.Status == "Hired")
        {
            var recruitment = await _db.Recruitments
                .FirstOrDefaultAsync(r => r.Id == jobApplication.RecruitmentId);
            if (recruitment is null)
                return;
            if (recruitment.NumberOfVacancies > 0)
            {
                recruitment.NumberOfVacancies--;
            }
        }
        jobApplicationFromDb.Status = jobApplication.Status;
    }

    public async Task<IDictionary<string, int>> GetMonthlyJobApplications()
    {
        var oneYearAgo = DateTime.Now.AddYears(-1);
        var jobApplications = await _db.JobApplications
            .Where(ja => ja.DateTime >= oneYearAgo)
            .GroupBy(ja => new { ja.DateTime.Year, ja.DateTime.Month })
            .Select(g => new
            {
                YearMonth = new DateTime(g.Key.Year, g.Key.Month, 1),
                Total = g.Count()
            })
            .ToDictionaryAsync(
                x => x.YearMonth.ToString("MMM"),
                x => x.Total
            );
        var allMonths = new[] 
        { 
            "Jan", "Feb", "Mar", "Apr", "May", "Jun", 
            "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        };
        
        return allMonths.ToDictionary(
            month => month,
            month => jobApplications.TryGetValue(month, out var value) ? value : 0
        );
    }
}