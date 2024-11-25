using Cores.DataService.Repository.IRepository;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Web.Areas.Accounting.Controllers;

[Area("Accounting")]
[Authorize(Roles = SD.ACCOUNTING_ROLE + "," + SD.ADMIN_ROLE)]
public class PayrollController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public PayrollController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var payrolls = await _unitOfWork.Payroll.GetAll(includeProperties:"Employee");
        return View(payrolls);
    }

    public async Task<IActionResult> Upsert(int? id)
    {
        var payrollVm = new PayrollVm
        {
            Payroll = new()
        };
        await FillSelectionData(payrollVm);
        if (id is null or 0)
        {
            return View(payrollVm);
        }
        
        var payroll = await _unitOfWork.Payroll.Get(p => p.Id == id);
        if (payroll is null)
        {
            return NotFound();
        }

        payrollVm.Payroll = payroll;
        return View(payrollVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(PayrollVm payrollVm)
    {
        if (!ModelState.IsValid)
        {
            await FillSelectionData(payrollVm);
            return View(payrollVm);
        }

        if (payrollVm.Payroll.Id == 0)
        {
            await _unitOfWork.Payroll.Add(payrollVm.Payroll);
            await MakeExpiredIfNeeded(payrollVm.Payroll.EmployeeId);
            TempData["success"] = "Payroll created successfully";
        }
        else
        {
            await _unitOfWork.Payroll.Update(payrollVm.Payroll);
            TempData["success"] = "Payroll updated successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var payroll = await _unitOfWork.Payroll.Get(p => p.Id == id);
        if (payroll is null)
        {
            return NotFound();
        }

        _unitOfWork.Payroll.Remove(payroll);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Payroll deleted successfully";
        return RedirectToAction(nameof(Index));
    }

    private async Task FillSelectionData(PayrollVm payrollVm)
    {
        var employees = await _unitOfWork.ApplicationUser.GetAll();
        payrollVm.Employees = employees.Select(e => new SelectListItem
        {
            Text = $"{e.FirstName} {e.LastName}",
            Value = e.Id
        }).ToList();
    }
    
    private async Task MakeExpiredIfNeeded(string employeeId) 
    {
        var employeeDeductions = await _unitOfWork.EmployeeDeduction.GetAll(
            ed => ed.EmployeeId ==employeeId &&
                  ed.Status == "One-Time" ||  ed.Status == "Active"
        );
        var employeeBenefits = await _unitOfWork.EmployeeBenefit.GetAll(
            eb => eb.EmployeeId == employeeId &&
                eb.Status == "One-Time" ||  eb.Status == "Active"
        );
        foreach (var employeeDeduction in employeeDeductions)
        {
            if (employeeDeduction.Status == "One-Time")
            {
                employeeDeduction.Status = "Expired";
                continue;
            }
            var today = DateTime.Now;
            if (employeeDeduction.EndDate < today)
            {
                employeeDeduction.Status = "Expired";
            }
        }
        foreach (var employeeBenefit in employeeBenefits)
        {
            if (employeeBenefit.Status == "One-Time")
            {
                employeeBenefit.Status = "Expired";
                continue;
            }
            var today = DateTime.Now;
            if (employeeBenefit.EndDate < today)
            {
                employeeBenefit.Status = "Expired";
            }        
        }
    }

    #region API CALL
    
    [HttpGet]
    public async Task<IActionResult> GetEmployee(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("Employee ID is required");
        }
        var employee = await _unitOfWork.ApplicationUser.Get(e => e.Id == id,
            includeProperties:"EmployeeDeductions.Deduction,EmployeeBenefits.Benefit");

        if (employee is null)
        {
            return NotFound("Employee not found");
        }
        
        var leaves = await _unitOfWork
            .UnpaidLeaveDeduction.GetAll(l => l.ApplicationUserId == employee.Id);

        var leavesDeductions = leaves
            .Where(l => l.DateTime.Month == DateTime.Now.Month)
            .Sum(d => d.Deduction);
        
        var totalDeductions = employee.EmployeeDeductions
            .Where(ed => (ed.Status == "Active" && ed.EndDate >= DateTime.Now) || ed.Status == "One-time")
            .Sum(d => d.Deduction.Amount);

        var totalBenefits = employee.EmployeeBenefits
            .Where(eb => (eb.Status == "Active" && eb.EndDate >= DateTime.Now) || eb.Status == "One-time")
            .Sum(b => b.Benefit.Amount);

        var response = new
        {
            employee.Salary,
            Deductions = totalDeductions + leavesDeductions,
            Benefits = totalBenefits
        };
        
        return Json(response);
    }
    
    #endregion
}