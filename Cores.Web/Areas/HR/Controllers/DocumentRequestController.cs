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
public class DocumentRequestController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public DocumentRequestController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var requests = await _unitOfWork.DocumentRequest.GetAll(
            includeProperties: "Employee,ApprovedBy");
        return View(requests);
    }

    public async Task<IActionResult> Upsert(int? id)
    {
        var requestVm = new DocumentRequestVm
        {
            DocumentTypes =
            [
                new() { Text = "Employment Certificate", Value = "Employment Certificate" },
                new() { Text = "Salary Certificate", Value = "Salary Certificate" },
                new() { Text = "Experience Letter", Value = "Experience Letter" }
            ]
        };

        if (id is null or  0)
        {
            requestVm.DocumentRequest = new()
            {
                ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!
            };
            return View(requestVm);
        }
        
        var documentRequest = await _unitOfWork.DocumentRequest.Get(
            r => r.Id == id, 
            includeProperties: "Employee,ApprovedBy");
        if (documentRequest is null)
        {
            return NotFound();
        }
        requestVm.DocumentRequest = documentRequest;
        return View(requestVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(DocumentRequestVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        if (vm.DocumentRequest.Id is 0)
        {
            await _unitOfWork.DocumentRequest.Add(vm.DocumentRequest);
            TempData["success"] = "Request submitted successfully";
        }
        else
        {
            await _unitOfWork.DocumentRequest.Update(vm.DocumentRequest);
            TempData["success"] = "Request updated successfully";
        }
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Answer(int id, bool response)
    {
        var request = await _unitOfWork.DocumentRequest.Get(r => r.Id == id);
        if (request is null)
            return NotFound();

        request.ApprovedById = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        request.ApprovalDate = DateTime.Now;
        request.Status = response ? "Approved" : "Rejected";
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
}