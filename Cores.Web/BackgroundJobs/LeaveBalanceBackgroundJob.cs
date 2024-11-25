using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;

namespace Cores.Web.BackgroundJobs;

public class LeaveBalanceBackgroundJob
{
    private readonly IUnitOfWork _unitOfWork;

    public LeaveBalanceBackgroundJob(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task UpdateLeaveBalances()
    {
        var employees = await _unitOfWork.ApplicationUser.GetAll();
        var today = DateTime.Now.Date;

        foreach (var employee in employees)
        {
            if (employee is not { AnnualLeaveEntitlement: not null, StartAt: not null }) continue;
            var startDate = employee.StartAt.Value.Date;
            var yearsEmployed = (today - startDate).Days / 365.25;
            var currentYearStartDate = startDate.AddYears((int)yearsEmployed);

            // Check if we need to reset the balance
            if (employee.ResetAnnualLeave && today == currentYearStartDate)
            {
                employee.AnnualLeaveBalance = 0;
            }

            // Calculate daily accrual
            var annualEntitlement = employee.AnnualLeaveEntitlement.Value;
            var monthlyAccrual = annualEntitlement / 12.0;
            var daysInCurrentMonth = DateTime.DaysInMonth(today.Year, today.Month);
            var dailyAccrual = monthlyAccrual / daysInCurrentMonth;

            // Update the balance
            employee.AnnualLeaveBalance += dailyAccrual;

            // Round to two decimal places
            employee.AnnualLeaveBalance = Math.Round(employee.AnnualLeaveBalance, 2);
        }

        await _unitOfWork.SaveAsync();
    }

    public async Task DeductLeaveBalances()
    {
        var employees = await _unitOfWork.ApplicationUser.GetAll(includeProperties: "LeaveRequests");
        var today = DateTime.Now.Date;

        // Fetch all holidays once outside the loop
        var holidays = await _unitOfWork.Holiday.GetAll();

        foreach (var employee in employees)
        {
            foreach (var leaveRequest in employee.LeaveRequests)
            {
                if (leaveRequest.StartDate != today || leaveRequest.IsDeducted) continue;
                if (leaveRequest is not { ManagerResponse: not null, HrResponse: not null } ||
                    !leaveRequest.ManagerResponse.Value || !leaveRequest.HrResponse.Value) continue;

                // Calculate the leave dates
                var leaveDates = Enumerable.Range(0, leaveRequest.NumberOfDays)
                    .Select(offset => leaveRequest.StartDate.AddDays(offset))
                    .ToList();

                // Filter out any dates that match holidays or specific excluded days of the week
                var holidayDays = leaveDates
                    .Count(date => holidays.Any(h =>
                        (h.StartAt.HasValue && h.StartAt.Value.Date == date) ||
                        (h.EndAt.HasValue && date >= h.StartAt && date <= h.EndAt) ||
                        (h.DayOfWeek.HasValue && h.DayOfWeek.Value == date.DayOfWeek)));

                // Adjust the leave days by excluding holiday days
                var effectiveLeaveDays = leaveRequest.NumberOfDays - holidayDays;
                if (effectiveLeaveDays <= 0) continue; // If all days are holidays, skip deduction

                if (leaveRequest.IsPayed)
                {
                    var employeeLeaveBalance = await _unitOfWork.EmployeeLeaveBalance.Get(
                        elb => elb.EmployeeId == leaveRequest.EmployeeId &&
                               elb.LeaveTypeId == leaveRequest.LeaveTypeId);

                    employee.AnnualLeaveBalance -= effectiveLeaveDays;
                    if (employeeLeaveBalance != null)
                    {
                        employeeLeaveBalance.DaysUsed += effectiveLeaveDays;
                    }
                }
                else
                {
                    var workingDays = employee.WorkingDaysInMonth;
                    if (workingDays is null or 0) continue;

                    var dailyRate = employee.Salary / workingDays;
                    var deduction = dailyRate * effectiveLeaveDays;

                    var unpaidLeaveDeduction = new UnpaidLeaveDeduction
                    {
                        Deduction = (decimal)deduction!,
                        DateTime = DateTime.Now,
                        ApplicationUserId = employee.Id
                    };
                    await _unitOfWork.UnpaidLeaveDeduction.Add(unpaidLeaveDeduction);
                }

                leaveRequest.IsDeducted = true;
            }

            await _unitOfWork.SaveAsync();
        }
    }
}