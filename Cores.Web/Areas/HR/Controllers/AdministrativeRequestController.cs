using System.Security.Claims;
using Cores.DataService.Repository.IRepository;
using Cores.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Web.Areas.HR.Controllers;

[Area("HR")]
[Authorize]
public class AdministrativeRequestController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public AdministrativeRequestController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var requests = await _unitOfWork.AdministrativeRequest.GetAll(
            includeProperties: "Employee,ApprovedBy");
        return View(requests);
    }

    public async Task<IActionResult> Upsert(int? id)
    {
        var requestVm = new AdministrativeRequestVm
        {
            RequestTypes = new List<SelectListItem>
            {
                new() { Text = "ID Card", Value = "ID Card" },
                new() { Text = "Access Card", Value = "Access Card" },
                new() { Text = "Parking Permit", Value = "Parking Permit" }
            }
        };

        if (id is null or 0)
        {
            requestVm.AdministrativeRequest = new()
            {
                RequiredDate = DateTime.Now.Date.AddDays(1),
                IsReplacement = false
            };
            requestVm.AdministrativeRequest.ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return View(requestVm);
        }

        var administrativeRequest = await _unitOfWork.AdministrativeRequest.Get(
            r => r.Id == id,
            includeProperties: "Employee,ApprovedBy");

        if (administrativeRequest is null)
            return NotFound();

        requestVm.AdministrativeRequest = administrativeRequest;
        return View(requestVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(AdministrativeRequestVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        if (vm.AdministrativeRequest.RequiredDate < DateTime.Now.Date)
        {
            ModelState.AddModelError("Request.RequiredDate", "Required date cannot be in the past");
            return View(vm);
        }

        if (vm.AdministrativeRequest.IsReplacement && string.IsNullOrWhiteSpace(vm.AdministrativeRequest.ReplacementReason))
        {
            ModelState.AddModelError("Request.ReplacementReason", "Replacement reason is required");
            return View(vm);
        }

        if (vm.AdministrativeRequest.Id is 0)
        {
            await _unitOfWork.AdministrativeRequest.Add(vm.AdministrativeRequest);
            TempData["success"] = "Administrative request submitted successfully";
        }
        else
        {
            await _unitOfWork.AdministrativeRequest.Update(vm.AdministrativeRequest);
            TempData["success"] = "Administrative request updated successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ApproveRequest(int id)
    {
        var request = await _unitOfWork.AdministrativeRequest.Get(r => r.Id == id);
        if (request == null)
            return NotFound();

        var claimsIdentity = (ClaimsIdentity)User.Identity!;
        request.ApprovedById = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        request.ApprovalDate = DateTime.Now;
        request.Status = "Approved";

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
}