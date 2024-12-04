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
    private readonly IWebHostEnvironment _webHostEnvironment;


    public BenefitsRequestController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
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

    public async Task<IActionResult> Answer(int id, bool response)
    {
        var request = await _unitOfWork.BenefitsRequest.Get(r => r.Id == id);
        if (request is null)
            return NotFound();

        request.ApprovedById = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        request.ApprovalDate = DateTime.Now;
        request.Status = response ? "Approved" : "Rejected";
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> DownloadDocument(int id)
    {
        var archive = await _unitOfWork.BenefitsRequest.Get(a => a.Id == id);
        if (archive is null || string.IsNullOrEmpty(archive.SupportingDocuments))
            return NotFound();

        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, archive.SupportingDocuments.TrimStart('\\'));
        if (!System.IO.File.Exists(filePath))
            return NotFound();

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(fileBytes, "application/octet-stream", Path.GetFileName(filePath));
    }
    
}