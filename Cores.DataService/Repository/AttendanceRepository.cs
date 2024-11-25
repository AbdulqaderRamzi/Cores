using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class AttendanceRepository : Repository<Attendance>,IAttendanceRepository
{
    private readonly ApplicationDbContext _db;
    
    public AttendanceRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Attendance attendance)
    {
        var attendanceFromDb = await _db.Attendances.FirstOrDefaultAsync(a => a.Id == attendance.Id);
        if (attendanceFromDb is null)
            return;
        attendanceFromDb.Date = attendance.Date;
        attendanceFromDb.TimeIn = attendance.TimeIn;
        attendanceFromDb.TimeOut = attendance.TimeOut;
        attendanceFromDb.Status = attendance.Status;
        attendanceFromDb.EmployeeId = attendanceFromDb.EmployeeId;
    }

    public async Task<IDictionary<string, int>> GetMonthlyLateEmployees()
    {
        var oneYearAgo = DateOnly.FromDateTime(DateTime.Now).AddYears(-1);
        var lateEmployees = await _db.Attendances
            .Where(a => !a.IsPresent && a.Date >= oneYearAgo)
            .GroupBy(a => new { a.Date.Year, a.Date.Month })
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
            month => lateEmployees.TryGetValue(month, out var value) ? value : 0
        );
    }
}