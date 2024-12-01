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
public class BenefitsRequestController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public BenefitsRequestController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var requests = await _unitOfWork.BenefitsRequest.GetAll(
            includeProperties: "Employee,ApprovedBy");
        return View(requests);
    }

    public async Task<IActionResult> Upsert(int? id)
    {
        var requestVm = new BenefitsRequestVm
        {
            BenefitTypes = new List<SelectListItem>
            {
                new() { Text = "Health Insurance", Value = "Health Insurance" },
                new() { Text = "Travel Allowance", Value = "Travel Allowance" }
            },
            Relationships = new List<SelectListItem>
            {
                new() { Text = "Spouse", Value = "Spouse" },
                new() { Text = "Child", Value = "Child" },
                new() { Text = "Parent", Value = "Parent" }
            }
        };

        if (id is null or 0)
        {
            requestVm.BenefitsRequest = new()
            {
                EffectiveDate = DateTime.Now.Date.AddDays(1)
            };
            requestVm.BenefitsRequest.ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return View(requestVm);
        }

        var benefitsRequest= await _unitOfWork.BenefitsRequest.Get(
            r => r.Id == id,
            includeProperties: "Employee,ApprovedBy");
        
        if (benefitsRequest is null)
            return NotFound();
        
        requestVm.BenefitsRequest = benefitsRequest;
        return View(requestVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(BenefitsRequestVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        if (vm.BenefitsRequest.EffectiveDate < DateTime.Now.Date)
        {
            ModelState.AddModelError("Request.EffectiveDate", "Effective date cannot be in the past");
            return View(vm);
        }

        if (vm.BenefitsRequest.Id is 0)
        {
            await _unitOfWork.BenefitsRequest.Add(vm.BenefitsRequest);
            TempData["success"] = "Benefits request submitted successfully";
        }
        else
        {
            await _unitOfWork.BenefitsRequest.Update(vm.BenefitsRequest);
            TempData["success"] = "Benefits request updated successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ApproveRequest(int id)
    {
        var request = await _unitOfWork.BenefitsRequest.Get(r => r.Id == id);
        if (request is null)
            return NotFound();

        var claimsIdentity = (ClaimsIdentity)User.Identity!;
        request.ApprovedById = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        request.ApprovalDate = DateTime.Now;
        request.Status = "Approved";

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
}