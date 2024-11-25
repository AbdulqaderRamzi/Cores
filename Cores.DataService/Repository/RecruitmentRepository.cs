using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class RecruitmentRepository : Repository<Recruitment>, IRecruitmentRepository
{
    private readonly ApplicationDbContext _db;

    public RecruitmentRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Recruitment recruitment)
    {
        var recruitmentFromDb = await _db.Recruitments.FirstOrDefaultAsync(r => r.Id == recruitment.Id);
        if (recruitmentFromDb is null)
            return;
        recruitmentFromDb.JobTitle = recruitment.JobTitle;
        recruitmentFromDb.DepartmentId = recruitment.DepartmentId;
        recruitmentFromDb.NumberOfVacancies = recruitment.NumberOfVacancies;
        recruitmentFromDb.JobDescription = recruitment.JobDescription;
        recruitmentFromDb.PostingDate = recruitment.PostingDate;
        recruitmentFromDb.ClosingDate = recruitment.ClosingDate;
    }
}