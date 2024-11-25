using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.HR.Controllers;

[Area("HR")]
[Authorize(Roles = SD.HR_ROLE + "," + SD.ADMIN_ROLE)]
public class DeductionController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public DeductionController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IActionResult> Index()
    {
        var deductions = await _unitOfWork.Deduction.GetAll();
        return View(deductions);
    }
    
    
    public async Task<IActionResult> Upsert(int? id)
    {
        if (id is null or 0)
            return View(new Deduction());
        var deduction = await _unitOfWork.Deduction.Get(b => b.Id == id);
        return View(deduction);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(Deduction deduction)
    {
        if (!ModelState.IsValid)
            return View(deduction);
        if (deduction.Id is 0)
        {
            await _unitOfWork.Deduction.Add(deduction);
            TempData["success"] = "Deduction added successfully";
        }
        else
        {
            await _unitOfWork.Deduction.Update(deduction);
            TempData["success"] = "Deduction updated successfully";
        }
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return NotFound();
        var benefit = await _unitOfWork.Deduction.Get(t => t.Id == id);
        if (benefit is null)
            return NotFound();
        _unitOfWork.Deduction.Remove(benefit);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Deduction deleted successfully";
        return RedirectToAction(nameof(Index));
    }
}