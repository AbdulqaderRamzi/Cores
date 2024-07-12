using Cores.DataService.Repository.IRepository;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Web.Areas.HR.Controllers;

[Area("HR")]
[Authorize(Roles = SD.HR_ROLE + "," + SD.ADMIN_ROLE)]
public class LeaveRequestController : Controller
{

    private readonly IUnitOfWork _unitOfWork;

    public LeaveRequestController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var leaveRequests = await _unitOfWork.LeaveRequest.GetAll(includeProperties: "Employee,LeaveType");
        return View(leaveRequests);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        var leaveRequestVm = new LeaveRequestVm();
        leaveRequestVm.LeaveRequest = new();

        await FillSelectionData(leaveRequestVm);

        if (id is 0)
        {
            return View(leaveRequestVm);
        }

        var leaveRequest = await _unitOfWork.LeaveRequest.Get(lr => lr.Id == id);
        if (leaveRequest is null)
            return NotFound();
        leaveRequestVm.LeaveRequest = leaveRequest;
        return View(leaveRequestVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(LeaveRequestVm leaveRequestVm)
    {
        if (!ModelState.IsValid)
        {
            await FillSelectionData(leaveRequestVm);
            return View(leaveRequestVm);
        }

        if (leaveRequestVm.LeaveRequest.Id is 0)
        {
            await _unitOfWork.LeaveRequest.Add(leaveRequestVm.LeaveRequest);
            TempData["success"] = "Leave request added successfully";
        }
        else
        {
            await _unitOfWork.LeaveRequest.Update(leaveRequestVm.LeaveRequest);
            TempData["success"] = "Leave request updated successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (id is 0)
            return BadRequest();
        var leaveRequest = await _unitOfWork.LeaveRequest.Get(lr => lr.Id == id);
        if (leaveRequest is null)
            return NotFound();
        _unitOfWork.LeaveRequest.Remove(leaveRequest);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task FillSelectionData(LeaveRequestVm leaveRequestVm)
    {
        var employees = await _unitOfWork.ApplicationUser.GetAll();
        leaveRequestVm.Employees = employees.Select(e => new SelectListItem
        {
            Text = $"{e.FirstName} {e.LastName}",
            Value = e.Id
        }).ToList();
        
        var leaveTypes = await _unitOfWork.LeaveType.GetAll();
        leaveRequestVm.LeaveTypes = leaveTypes.Select(lt => new SelectListItem
        {
            Text = lt.Name,
            Value = lt.Id.ToString()
        }).ToList();
    }
}