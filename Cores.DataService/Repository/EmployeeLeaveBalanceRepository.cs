using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class EmployeeLeaveBalanceRepository : Repository<EmployeeLeaveBalance>, IEmployeeLeaveBalanceRepository
{
    private readonly ApplicationDbContext _db;

    public EmployeeLeaveBalanceRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }
    
    public async Task Update(EmployeeLeaveBalance employeeLeaveBalance)
    {
        var employeeLeaveBalanceFromDb = await _db.EmployeeLeaveBalances.FirstOrDefaultAsync(
            elb => elb.Id == employeeLeaveBalance.Id
        );
        if (employeeLeaveBalanceFromDb is null) 
            return;
        employeeLeaveBalanceFromDb.EmployeeId = employeeLeaveBalance.EmployeeId;
        employeeLeaveBalanceFromDb.LeaveTypeId = employeeLeaveBalance.LeaveTypeId;
        employeeLeaveBalanceFromDb.CurrentDate = employeeLeaveBalance.CurrentDate;
        employeeLeaveBalanceFromDb.EndDate = employeeLeaveBalance.EndDate;
        employeeLeaveBalanceFromDb.DaysUsed = employeeLeaveBalance.DaysUsed;
    }
}