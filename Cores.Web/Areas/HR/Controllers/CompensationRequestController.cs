using System.Security.Claims;
using Cores.DataService.Repository.IRepository;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Web.Areas.HR.Controllers;

[Area("HR")]
[Authorize]
public class CompensationRequestController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CompensationRequestController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var requests = await _unitOfWork.CompensationRequest.GetAll(
            includeProperties: "Employee,ApprovedBy");
        return View(requests);
    }

    public async Task<IActionResult> Upsert(int? id)
    {
        var requestVm = new CompensationRequestVm
        {
            CompensationTypes =
            [
                new() { Text = "Salary Review", Value = "Salary Review" },
                new() { Text = "Overtime Claims", Value = "Overtime Claims" }
            ]
        };

        if (id is null or 0)
        {
            requestVm.CompensationRequest = new()
            {
                PeriodStart = DateTime.Now.Date,
                PeriodEnd = DateTime.Now.Date
            };
            requestVm.CompensationRequest.ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return View(requestVm);
        }

        var compensationRequest = await _unitOfWork.CompensationRequest.Get(
            r => r.Id == id,
            includeProperties: "Employee,ApprovedBy");
        if (compensationRequest is null)
        {
            return NotFound();
        }
        requestVm.CompensationRequest = compensationRequest;

        return View(requestVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(CompensationRequestVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        if (vm.CompensationRequest.PeriodEnd < vm.CompensationRequest.PeriodStart)
        {
            ModelState.AddModelError("Request.PeriodEnd", "End date cannot be before start date");
            return View(vm);
        }

        if (vm.CompensationRequest.Id == 0)
        {
            await _unitOfWork.CompensationRequest.Add(vm.CompensationRequest);
            TempData["success"] = "Request submitted successfully";
        }
        else
        {
            await _unitOfWork.CompensationRequest.Update(vm.CompensationRequest);
            TempData["success"] = "Request updated successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ApproveRequest(int id)
    {
        var request = await _unitOfWork.CompensationRequest.Get(r => r.Id == id);
        if (request is null)
            return NotFound();

        request.ApprovedById = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        request.ApprovalDate = DateTime.Now;
        request.Status = "Approved";

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
}