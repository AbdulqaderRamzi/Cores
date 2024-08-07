using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Accounting.Controllers;

[Area("Accounting")]
[Authorize(Roles = SD.ACCOUNTING_ROLE + "," + SD.ADMIN_ROLE)]
public class TaxController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public TaxController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var taxRates = await _unitOfWork.Tax.GetAll();
        return View(taxRates);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        if (id is 0)
            return View(new Tax());
            
        var taxRate = await _unitOfWork.Tax.Get(t => t.Id == id);
        if (taxRate == null) 
            return NotFound();
        return View(taxRate);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(Tax tax)
    {
        if (!ModelState.IsValid)
        {
            return View(tax);
        }
        if (tax.Id is 0)
        {
            await _unitOfWork.Tax.Add(tax);
            TempData["success"] = "Tax Rate created successfully";
        }
        else
        {
            await _unitOfWork.Tax.Update(tax);
            TempData["success"] = "Tax Rate updated successfully";
        }
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
        return View(tax);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return NotFound();
        var tax = await _unitOfWork.Tax.Get(t => t.Id == id);
        if (tax is null)
            return NotFound();
        _unitOfWork.Tax.Remove(tax);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Tax deleted successfully";
        return RedirectToAction(nameof(Index));
    }
}