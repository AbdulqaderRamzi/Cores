using Cores.DataService.Repository.IRepository;
using Cores.Models.CRM;
using Cores.Models.HR;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.HR.Controllers;

[Area("HR")]
[Authorize(Roles = SD.HR_ROLE + "," + SD.ADMIN_ROLE)]
public class LeaveTypeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public LeaveTypeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var leaveTypes = await _unitOfWork.LeaveType.GetAll();
        return View(leaveTypes);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        if (id is 0)
        {
            return View(new LeaveType());
        }

        var leaveType = await _unitOfWork.LeaveType.Get(lt => lt.Id == id);
        if (leaveType is null)
            return NotFound();
        return View(leaveType);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(LeaveType leaveType)
    {
        if (!ModelState.IsValid)
            return View(leaveType);
        if (leaveType.Id is 0)
        {
            await _unitOfWork.LeaveType.Add(leaveType);
            TempData["success"] = "Leave Type added successfully";
        }
        else
        {
            await _unitOfWork.LeaveType.Update(leaveType);
            TempData["success"] = "Leave Type updated successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Delete(int id)
    {
        if (id is 0)
            return BadRequest();
        var leaveType = await _unitOfWork.LeaveType.Get(lt => lt.Id == id);
        if (leaveType is null)
            return NotFound();
        _unitOfWork.LeaveType.Remove(leaveType);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
}