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
}