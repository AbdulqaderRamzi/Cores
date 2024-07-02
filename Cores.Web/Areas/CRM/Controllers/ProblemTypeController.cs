using Cores.DataService.Repository.IRepository;
using Cores.Models.CRM;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.CRM.Controllers;

[Area("CRM")]
[Authorize(Roles = SD.CRM_ROLE + "," + SD.ADMIN_ROLE)]
public class ProblemTypeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ProblemTypeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var problemTypes = await _unitOfWork.ProblemType.GetAll();
        return View(problemTypes);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        if (id is 0)
        {
            return View(new ProblemType());
        }

        var problemType = await _unitOfWork.ProblemType.Get(pt => pt.Id == id);
        if (problemType is null)
            return NotFound();
        return View(problemType);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(ProblemType problemType)
    {
        if (!ModelState.IsValid)
            return View(problemType);
        if (problemType.Id is 0)
        {
            await _unitOfWork.ProblemType.Add(problemType);
            TempData["success"] = "Problem Type added successfully";
        }
        else
        {
            await _unitOfWork.ProblemType.Update(problemType);
            TempData["success"] = "Problem Type updated successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Delete(int id)
    {
        if (id is 0)
            return BadRequest();
        var problemType = await _unitOfWork.ProblemType.Get(pt => pt.Id == id);
        if (problemType is null)
            return NotFound();
        _unitOfWork.ProblemType.Remove(problemType);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
}