using Cores.DataService.Repository.IRepository;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Web.Areas.HR.Controllers;

[Area("HR")]
[Authorize(Roles = SD.HR_ROLE + "," + SD.ADMIN_ROLE)]
public class SalaryController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public SalaryController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IActionResult> Index()
    {
        var salaries = await _unitOfWork.Salary.GetAll(includeProperties:"Employee");
        return View(salaries);
    }
    
    public async Task<IActionResult> Upsert(int id)
    {
        var employees = await _unitOfWork.ApplicationUser.GetAll();
        var employeesList = employees.Select(e => new SelectListItem
        {
            Text = $"{e.FirstName} {e.LastName}",
            Value = e.Id
        }).ToList();
        
        var salaryVm = new SalaryVm
        {
            Salary = new(),
            Employees = employeesList
        };
    
        if (id is 0)
        {
            return View(salaryVm);
        }

        var salary = await _unitOfWork.Salary.Get(r => r.Id == id, includeProperties:"Employee");
        if (salary is null)
            return NotFound();
        salaryVm.Salary = salary;
        return View(salaryVm);
    }
    
    [HttpPost]
    public async Task<IActionResult> Upsert(SalaryVm salaryVm)
    {
        if (!ModelState.IsValid)
        {
            var employees = await _unitOfWork.ApplicationUser.GetAll();
            var employeesList = employees.Select(e => new SelectListItem
            {
                Text = $"{e.FirstName} {e.LastName}",
                Value = e.Id
            }).ToList();
            salaryVm.Employees = employeesList;
            
            return View(salaryVm);
            
        }
        if (salaryVm.Salary.Id is 0)
        {
            await _unitOfWork.Salary.Add(salaryVm.Salary);
            TempData["success"] = "Salary added successfully";
        }
        else
        {
            await _unitOfWork.Salary.Update(salaryVm.Salary);
            TempData["success"] = "Salary updated successfully";
        }
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        if (id is 0)
            return BadRequest();
        var salary = await _unitOfWork.Salary.Get(s => s.Id == id);
        if (salary is null)
            return NotFound();
        _unitOfWork.Salary.Remove(salary);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
}