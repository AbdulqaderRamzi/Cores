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
public class EmployeeTrainingController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeTrainingController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var employeeTrainings = await _unitOfWork.EmployeeTraining.GetAll(includeProperties: "Employee,Training");
        return View(employeeTrainings);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        var employeeTrainingVm = new EmployeeTrainingVm
        {
            EmployeeTraining = new EmployeeTraining(),
        };

        await FillSelectionData(employeeTrainingVm);

        if (id == 0)
            return View(employeeTrainingVm);

        employeeTrainingVm.EmployeeTraining = await _unitOfWork.EmployeeTraining.Get(et => et.Id == id);
        if (employeeTrainingVm.EmployeeTraining == null)
            return NotFound();

        return View(employeeTrainingVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(EmployeeTrainingVm employeeTrainingVm)
    {
        if (!ModelState.IsValid)
        {
            await FillSelectionData(employeeTrainingVm);
            return View(employeeTrainingVm);
        }

        if (employeeTrainingVm.EmployeeTraining.Id == 0)
        {
            await _unitOfWork.EmployeeTraining.Add(employeeTrainingVm.EmployeeTraining);
            TempData["success"] = "Employee training created successfully";
        }
        else
        {
            await _unitOfWork.EmployeeTraining.Update(employeeTrainingVm.EmployeeTraining);
            TempData["success"] = "Employee training updated successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (id is 0)
            return BadRequest();
        var employeeTraining = await _unitOfWork.EmployeeTraining.Get(et => et.Id == id);
        if (employeeTraining is null)
            return NotFound();
        _unitOfWork.EmployeeTraining.Remove(employeeTraining);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task FillSelectionData(EmployeeTrainingVm employeeTrainingVm)
    {
        var employees = await _unitOfWork.ApplicationUser.GetAll();
        employeeTrainingVm.Employees = employees.Select(e => new SelectListItem
        {
            Text = $"{e.FirstName} {e.LastName}",
            Value = e.Id
        });
        var trainings = await _unitOfWork.Training.GetAll();
        employeeTrainingVm.Trainings = trainings.Select(t => new SelectListItem
        {
            Text = t.Name,
            Value = t.Id.ToString()
        });
    }
}