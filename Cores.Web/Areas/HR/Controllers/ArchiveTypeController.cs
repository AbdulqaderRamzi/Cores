using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.HR.Controllers;


[Area("HR")]
[Authorize(Roles = SD.HR_ROLE + "," + SD.ADMIN_ROLE)]
public class ArchiveTypeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ArchiveTypeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var archiveTypes = await _unitOfWork.ArchiveType.GetAll();
        return View(archiveTypes);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        if (id is 0)
        {
            return View(new ArchiveType());
        }

        var archiveType = await _unitOfWork.ArchiveType.Get(lt => lt.Id == id);
        if (archiveType is null)
            return NotFound();
        return View(archiveType);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(ArchiveType archiveType)
    {
        if (!ModelState.IsValid)
            return View(archiveType);
        if (archiveType.Id is 0)
        {
            await _unitOfWork.ArchiveType.Add(archiveType);
            TempData["success"] = "Archive Type added successfully";
        }
        else
        {
            await _unitOfWork.ArchiveType.Update(archiveType);
            TempData["success"] = "Archive Type updated successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Delete(int id)
    {
        if (id is 0)
            return BadRequest();
        var leaveType = await _unitOfWork.ArchiveType.Get(at => at.Id == id);
        if (leaveType is null)
            return NotFound();
        _unitOfWork.ArchiveType.Remove(leaveType);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
}