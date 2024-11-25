using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class SalaryRepository : Repository<Salary>, ISalaryRepository
{
    private readonly ApplicationDbContext _db;

    public SalaryRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Salary salary)
    {
        var salaryFromDb = await _db.Salaries.FirstOrDefaultAsync(s => s.Id == salary.Id);
        if (salaryFromDb is null)
            return;
        salaryFromDb.BaseSalary = salary.BaseSalary;
        salaryFromDb.Bonuses = salary.Bonuses;
        salaryFromDb.Deductions = salary.Deductions;
        salaryFromDb.Deductions = salary.Deductions;
        salaryFromDb.EffectiveDate = salary.EffectiveDate;
        salaryFromDb.EmployeeId = salary.EmployeeId;
    }
}