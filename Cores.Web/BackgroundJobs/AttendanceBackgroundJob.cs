using Cores.DataService.Repository.IRepository;
using Cores.Models;
using Cores.Models.HR;

namespace Cores.Web.BackgroundJobs;

public class AttendanceBackgroundJob
{
    private readonly IUnitOfWork _unitOfWork;

    public AttendanceBackgroundJob(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CheckMissingAttendance()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var missingEmployees = await GetEmployeesWithMissingAttendance(today);

        foreach (var employee in missingEmployees)
        {
            // Log the missing attendance
            await _unitOfWork.Attendance.Add(new Attendance
            {
                EmployeeId = employee.Id,
                Date = today,
                IsPresent = false 
            });

            // Optionally, send a notification
            // await SendNotification(employee);
        }

        await _unitOfWork.SaveAsync();
    }

    private async Task<List<ApplicationUser>> GetEmployeesWithMissingAttendance(DateOnly date)
    {
        var allEmployees = await _unitOfWork.ApplicationUser.GetAll(includeProperties: "WorkSchedules");
        List<ApplicationUser> missingEmployees = [];

        foreach (var employee in allEmployees)
        {
            // Check if the employee should be working on this day
            var schedule = employee.WorkSchedules.FirstOrDefault(ws => ws.DayOfWeek == date.DayOfWeek);
            if (schedule is null) continue;
            // Check if there's an attendance record for this employee on this date
            var attendance = await _unitOfWork.Attendance.Get(a => 
                a.EmployeeId == employee.Id && 
                a.Date == date);

            if (attendance?.TimeIn is null || attendance.TimeOut is null)
            {
                // No attendance record found, add to missing employees list
                missingEmployees.Add(employee);
            }
        }
        return missingEmployees;
    }
}
