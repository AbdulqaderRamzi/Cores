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
public class TrainingController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public TrainingController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var trainings = await _unitOfWork.Training.GetAll(includeProperties: "Trainer");
        return View(trainings);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        var trainingVm = new TrainingVm
        {
            Training = new Training(),
        };

        await FillSelectionData(trainingVm);

        if (id == 0)
            return View(trainingVm);

        trainingVm.Training = await _unitOfWork.Training.Get(t => t.Id == id);
        if (trainingVm.Training == null)
            return NotFound();

        return View(trainingVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(TrainingVm trainingVm)
    {
        if (!ModelState.IsValid)
        {
            await FillSelectionData(trainingVm);
            return View(trainingVm);
        }

        if (trainingVm.Training.Id == 0)
        {
            await _unitOfWork.Training.Add(trainingVm.Training);
            TempData["success"] = "Training created successfully";
        }
        else
        {
            await _unitOfWork.Training.Update(trainingVm.Training);
            TempData["success"] = "Training updated successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (id is 0)
            return BadRequest();
        var training = await _unitOfWork.Training.Get(t => t.Id == id);
        if (training is null)
            return NotFound();
        _unitOfWork.Training.Remove(training);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task FillSelectionData(TrainingVm trainingVm)
    {
        var trainers = await _unitOfWork.ApplicationUser.GetAll();
        trainingVm.Employees = trainers.Select(t => new SelectListItem
        {
            Text = $"{t.FirstName} {t.LastName}",
            Value = t.Id
        }).ToList();
    }
}