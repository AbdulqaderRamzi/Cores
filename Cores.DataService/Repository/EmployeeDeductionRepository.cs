using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class EmployeeDeductionRepository : Repository<EmployeeDeduction>, IEmployeeDeductionRepository
{
    private readonly ApplicationDbContext _db;

    public EmployeeDeductionRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(EmployeeDeduction employeeDeduction)
    {
        var employeeDeductionFromDb = await _db.EmployeeDeductions.FirstOrDefaultAsync(eb => eb.Id == employeeDeduction.Id);
        if (employeeDeductionFromDb is null)
            return;
        employeeDeductionFromDb.EmployeeId = employeeDeduction.EmployeeId;
        employeeDeductionFromDb.DeductionId = employeeDeduction.DeductionId;
        employeeDeductionFromDb.StartDate = employeeDeduction.StartDate;
        employeeDeductionFromDb.EndDate = employeeDeduction.EndDate;
        employeeDeductionFromDb.Status = employeeDeduction.Status;
    }
}