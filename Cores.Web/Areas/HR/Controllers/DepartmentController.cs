using Cores.DataService.Repository.IRepository;
using Cores.Models;
using Cores.Models.HR;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Web.Areas.HR.Controllers;

[Area("HR")]
[Authorize(Roles = SD.CRM_ROLE + "," + SD.ADMIN_ROLE)]
public class DepartmentController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public DepartmentController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var departments = await _unitOfWork.Department.GetAll(includeProperties:"DepartmentHead");
        return View(departments);
    }
    
    public async Task<IActionResult> Upsert(int id)
    {
        var departmentVm = new DepartmentVm();
        var employees = await _unitOfWork.ApplicationUser.GetAll();
        List<ApplicationUser> employeesDepartment  = [];
        departmentVm.Employees = employees
            .Select(e =>
            {
                if (e.DepartmentId == id)
                {
                    employeesDepartment.Add(e);
                }
                return new SelectListItem
                {
                    Text = $"{e.FirstName} {e.LastName}",
                    Value = e.Id
                };
            })
            .ToList();

        departmentVm.EmployeesDepartment = employeesDepartment;
            
        if (id is 0)
        {
            departmentVm.Department = new Department();
            return View(departmentVm);
        }

        var department = await _unitOfWork.Department.Get(u => u.Id == id, includeProperties:"DepartmentHead,Employees.Position");
        if (department is null)
            return NotFound();
        departmentVm.Department = department;
        return View(departmentVm);
    }
    
    [HttpPost]
    public async Task<IActionResult> Upsert(DepartmentVm departmentVm)
    {
        if (!ModelState.IsValid)
        {
            var employees = await _unitOfWork.ApplicationUser.GetAll(/*includeProperties:"Position,Manager"*/);
            List<ApplicationUser> employeesDepartment  = [];
            departmentVm.Employees = employees
                .Select(e =>
                {
                    if (e.DepartmentId == departmentVm.Department.Id) 
                    {
                        employeesDepartment.Add(e);
                    }
                    return new SelectListItem
                    {
                        Text = $"{e.FirstName} {e.LastName}",
                        Value = e.Id
                    };
                })
                .ToList();

            departmentVm.EmployeesDepartment = employeesDepartment;         
            return View(departmentVm);
        }

        if (departmentVm.Department.Id is 0)
            await _unitOfWork.Department.Add(departmentVm.Department);
        else
            await _unitOfWork.Department.Update(departmentVm.Department);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return NotFound();
        var department = await _unitOfWork.Department.Get(d => d.Id == id,"Employees");
        if (department is null)
            return NotFound();
        if (department.Employees.Count is not 0)
        {
            TempData["error"] = "The Department has employees";
            return RedirectToAction(nameof(Index));
        }
        /*foreach (var employee in department.Employees)
        {
            employee.DepartmentId = null;
        }*/
        _unitOfWork.Department.Remove(department);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Department deleted successfully";
        return RedirectToAction(nameof(Index));
    }
}