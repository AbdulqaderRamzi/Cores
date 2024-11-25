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
public class EmployeeBenefitController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeBenefitController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IActionResult> Index()
    {
        var employeeBenefits = 
            await _unitOfWork.EmployeeBenefit.GetAll(includeProperties:"Employee,Benefit");
        return View(employeeBenefits);
    }
    
    public async Task<IActionResult> Upsert(int? id)
    {
        var employeeBenefitVm = new EmployeeBenefitVm
        {
            EmployeeBenefit = new()
        };
        await FillSelectionData(employeeBenefitVm);
        
        if (id is null or 0)
            return View(employeeBenefitVm);
        var employeeBenefit = await _unitOfWork.EmployeeBenefit.Get(eb => eb.Id == id);
        if (employeeBenefit is null)
            return NotFound();
        employeeBenefitVm.EmployeeBenefit = employeeBenefit;
        return View(employeeBenefitVm);
    }
    
    [HttpPost]
    public async Task<IActionResult> Upsert(EmployeeBenefitVm employeeBenefitVm)
    {
        if (!ModelState.IsValid)
        {
            await FillSelectionData(employeeBenefitVm);
            return View(employeeBenefitVm);
        }
        if (employeeBenefitVm.EmployeeBenefit.Id is 0)
        {
            await _unitOfWork.EmployeeBenefit.Add(employeeBenefitVm.EmployeeBenefit);
            TempData["success"] = "Employee benefit added successfully";
        }
        else
        {
            await _unitOfWork.EmployeeBenefit.Update(employeeBenefitVm.EmployeeBenefit);
            TempData["success"] = "Employee benefit updated successfully";
        }
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return NotFound();
        var employeeBenefit = await _unitOfWork.EmployeeBenefit.Get(eb => eb.Id == id);
        if (employeeBenefit is null)
            return NotFound();
        _unitOfWork.EmployeeBenefit.Remove(employeeBenefit);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Employee benefit deleted successfully";
        return RedirectToAction(nameof(Index));
    }

    private async Task FillSelectionData(EmployeeBenefitVm employeeBenefitVm)
    {
        var employees = await _unitOfWork.ApplicationUser.GetAll();
        var benefits = await _unitOfWork.Benefit.GetAll();
        employeeBenefitVm.Employees = employees.Select(e => new SelectListItem
        {
            Text = $"{e.FirstName} {e.LastName}",
            Value = e.Id
        }).ToList();
        employeeBenefitVm.Benefits = benefits.Select(b => new SelectListItem
        {
            Text = b.Name,
            Value = b.Id.ToString()
        }).ToList();
    }

}