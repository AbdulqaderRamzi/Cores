using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Web.Areas.HR.Controllers;

[Area("HR")]
[Authorize(Roles = SD.HR_ROLE + "," + SD.ADMIN_ROLE)]
public class EmployeeDeductionController : Controller
{
  private readonly IUnitOfWork _unitOfWork;

    public EmployeeDeductionController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IActionResult> Index()
    {
        var employeeBenefits = 
            await _unitOfWork.EmployeeDeduction.GetAll(includeProperties:"Employee,Deduction");
        return View(employeeBenefits);
    }
    
    public async Task<IActionResult> Upsert(int? id, int? attendanceId = null)
    {
        var employeeDeductionVm = new EmployeeDeductionVm
        {
            EmployeeDeduction = new()
        };
        await FillSelectionData(employeeDeductionVm);
        
        if (attendanceId is not null)
        {
            var attendance = await _unitOfWork.Attendance.Get(a => a.Id == attendanceId);
            if (attendance is null)
                return NotFound();
            employeeDeductionVm.EmployeeDeduction = new EmployeeDeduction
            {
                EmployeeId = attendance.EmployeeId!,
                StartDate = attendance.Date.ToDateTime(TimeOnly.MinValue),
                EndDate = attendance.Date.ToDateTime(TimeOnly.MinValue).AddDays(1),
                Status = "One-time"
            };
            employeeDeductionVm.IsFromAttendance = true;
            employeeDeductionVm.AttendanceId = attendance.Id;
        }
        
        if (id is null or 0)
            return View(employeeDeductionVm);
        
        var employeeDeduction = await _unitOfWork.EmployeeDeduction.Get(eb => eb.Id == id);
        if (employeeDeduction is null)
            return NotFound();
        employeeDeductionVm.EmployeeDeduction = employeeDeduction;
        return View(employeeDeductionVm);
    }
    
    [HttpPost]
    public async Task<IActionResult> Upsert(EmployeeDeductionVm employeeDeductionVm)
    {
        if (!ModelState.IsValid)
        {
            await FillSelectionData(employeeDeductionVm);
            return View(employeeDeductionVm);   
        }

        if (employeeDeductionVm.EmployeeDeduction.Id is 0)
        {
            await _unitOfWork.EmployeeDeduction.Add(employeeDeductionVm.EmployeeDeduction);
            TempData["success"] = "Employee deduction added successfully";
        }
        else
        {
            await _unitOfWork.EmployeeDeduction.Update(employeeDeductionVm.EmployeeDeduction);
            TempData["success"] = "Employee deduction updated successfully";
        }
        await _unitOfWork.SaveAsync();
        return employeeDeductionVm.IsFromAttendance ? 
             RedirectToAction("Ignore", "Attendance", new { id = employeeDeductionVm.AttendanceId }) :
             RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return NotFound();
        var employeeDeduction = await _unitOfWork.EmployeeDeduction.Get(eb => eb.Id == id);
        if (employeeDeduction is null)
            return NotFound();
        _unitOfWork.EmployeeDeduction.Remove(employeeDeduction);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Employee deduction deleted successfully";
        return RedirectToAction(nameof(Index));
    }

    private async Task FillSelectionData(EmployeeDeductionVm employeeDeductionVm)
    {
        var employees = await _unitOfWork.ApplicationUser.GetAll();
        var deductions = await _unitOfWork.Deduction.GetAll();
        employeeDeductionVm.Employees = employees.Select(e => new SelectListItem
        {
            Text = $"{e.FirstName} {e.LastName}",
            Value = e.Id
        }).ToList();
        employeeDeductionVm.Deductions = deductions.Select(b => new SelectListItem
        {
            Text = b.Name,
            Value = b.Id.ToString()
        }).ToList();
    }
}