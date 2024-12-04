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
    private readonly IWebHostEnvironment _webHostEnvironment;


    public CompensationRequestController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
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
    public async Task<IActionResult> Upsert(CompensationRequestVm vm, IFormFile? file)
    {
        if (!ModelState.IsValid)
            return View(vm);

        if (file is not null)
        {
            var wwwRootPath = _webHostEnvironment.WebRootPath;
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var documentsPath = Path.Combine(wwwRootPath, @"images\compensation-requests");
            await using (var fileStream = new FileStream(Path.Combine(documentsPath, fileName), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            if (!string.IsNullOrEmpty(vm.CompensationRequest.SupportingDocuments))
            {
                var oldDocumentPath = Path.Combine(wwwRootPath, vm.CompensationRequest.SupportingDocuments.TrimStart('\\'));
                if (System.IO.File.Exists(oldDocumentPath))
                {
                    System.IO.File.Delete(oldDocumentPath);
                }
            }
            vm.CompensationRequest.SupportingDocuments = @"\images\compensation-requests\" + fileName;
        }

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

    public async Task<IActionResult> Answer(int id, bool response)
    {
        var request = await _unitOfWork.CompensationRequest.Get(r => r.Id == id);
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
        var archive = await _unitOfWork.CompensationRequest.Get(a => a.Id == id);
        if (archive is null || string.IsNullOrEmpty(archive.SupportingDocuments))
            return NotFound();

        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, archive.SupportingDocuments.TrimStart('\\'));
        if (!System.IO.File.Exists(filePath))
            return NotFound();

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(fileBytes, "application/octet-stream", Path.GetFileName(filePath));
    }
}