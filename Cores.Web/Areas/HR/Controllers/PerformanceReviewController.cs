using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Web.Areas.HR.Controllers;

[Area("HR")]
[Authorize(Roles = SD.HR_ROLE + "," + SD.ADMIN_ROLE)]
public class PerformanceReviewController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public PerformanceReviewController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IActionResult> Index()
    {
        var performanceReviews = await _unitOfWork.PerformanceReview.GetAll(includeProperties: "Employee,Reviewer");
        return View(performanceReviews);
    }
    
    public async Task<IActionResult> Upsert(int id)
    {
        var performanceReviewVm = new PerformanceReviewVm
        {
            PerformanceReview = new PerformanceReview(),
        };
        await FillSelectionData(performanceReviewVm);

        if (id == 0)
            return View(performanceReviewVm);

        var performanceReview = await _unitOfWork.PerformanceReview.Get(pr => pr.Id == id);
        if (performanceReview is null)
            return NotFound();
        performanceReviewVm.PerformanceReview = performanceReview;
        return View(performanceReviewVm);
    }
    
    [HttpPost]
    public async Task<IActionResult> Upsert(PerformanceReviewVm performanceReviewVm)
    {
        if (!ModelState.IsValid)
        { 
            await FillSelectionData(performanceReviewVm);
            return View(performanceReviewVm);
        }
        
        if (performanceReviewVm.PerformanceReview.Id == 0)
        {
            await _unitOfWork.PerformanceReview.Add(performanceReviewVm.PerformanceReview);
            TempData["success"] = "Performance review created successfully";
        }
        else
        {
            await _unitOfWork.PerformanceReview.Update(performanceReviewVm.PerformanceReview);
            TempData["success"] = "Performance review updated successfully";
        }
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (id is 0)
            return BadRequest();
        var performanceReview = await _unitOfWork.PerformanceReview.Get(pr => pr.Id == id);
        if (performanceReview is null)
            return NotFound();
        _unitOfWork.PerformanceReview.Remove(performanceReview);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
    
    private async Task FillSelectionData(PerformanceReviewVm performanceReviewVm)
    {
        var employees = await _unitOfWork.ApplicationUser.GetAll();
        performanceReviewVm.Employees = employees.Select(t => new SelectListItem
        {
            Text = $"{t.FirstName} {t.LastName}",
            Value = t.Id
        }).ToList();
    }
}