using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.HR.Controllers;

[Area("HR")]
[Authorize(Roles = SD.HR_ROLE + "," + SD.ADMIN_ROLE)]
public class BenefitController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public BenefitController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IActionResult> Index()
    {
        var benefits = await _unitOfWork.Benefit.GetAll();
        return View(benefits);
    }
    
    
    public async Task<IActionResult> Upsert(int? id)
    {
        if (id is null or 0)
            return View(new Benefit());
        var benefit = await _unitOfWork.Benefit.Get(b => b.Id == id);
        return View(benefit);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(Benefit benefit)
    {
        if (!ModelState.IsValid)
            return View(benefit);
        if (benefit.Id is 0)
        {
            await _unitOfWork.Benefit.Add(benefit);
            TempData["success"] = "Benefits added successfully";
        }
        else
        {
            await _unitOfWork.Benefit.Update(benefit);
            TempData["success"] = "Benefits updated successfully";
        }
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return NotFound();
        var benefit = await _unitOfWork.Benefit.Get(t => t.Id == id);
        if (benefit is null)
            return NotFound();
        _unitOfWork.Benefit.Remove(benefit);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Benefits deleted successfully";
        return RedirectToAction(nameof(Index));
    }
}