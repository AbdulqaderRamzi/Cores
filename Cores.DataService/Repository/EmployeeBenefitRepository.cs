using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class EmployeeBenefitRepository : Repository<EmployeeBenefit>, IEmployeeBenefitRepository
{
    private readonly ApplicationDbContext _db;

    public EmployeeBenefitRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(EmployeeBenefit employeeBenefit)
    {
        var employeeBenefitFromDb = await _db.EmployeeBenefits.FirstOrDefaultAsync(eb => eb.Id == employeeBenefit.Id);
        if (employeeBenefitFromDb is null)
            return;
        employeeBenefitFromDb.EmployeeId = employeeBenefit.EmployeeId;
        employeeBenefitFromDb.BenefitId = employeeBenefit.BenefitId;
        employeeBenefitFromDb.StartDate = employeeBenefit.StartDate;
        employeeBenefitFromDb.EndDate = employeeBenefit.EndDate;
        employeeBenefitFromDb.Status = employeeBenefit.Status;
    }
}