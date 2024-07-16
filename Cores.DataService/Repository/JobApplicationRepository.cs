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
        jobApplicationFromDb.ApplicationDate = jobApplication.ApplicationDate;
        jobApplicationFromDb.Status = jobApplication.Status;
    }
}