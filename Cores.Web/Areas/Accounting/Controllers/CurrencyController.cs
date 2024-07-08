using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Accounting.Controllers;

[Area("Accounting")]
[Authorize(Roles = SD.ACCOUNTING_ROLE + "," + SD.ADMIN_ROLE)]
public class CurrencyController : Controller
{

    private readonly IUnitOfWork _unitOfWork;

    public CurrencyController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    
    public async Task<IActionResult> Index()
    {
        var currencies = await _unitOfWork.Currency.GetAll();
        return View(currencies);
    }

    public async Task<IActionResult> Upsert(int? id)
    {
        if (id is null or 0)
            return View(new Currency());
        var currency = await _unitOfWork.Currency.Get(u => u.Id == id);
        return View(currency);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(Currency currency)
    {
        if (!ModelState.IsValid)
            return View(currency);
        if (currency.Id is 0)
            await _unitOfWork.Currency.Add(currency);
        else
            await _unitOfWork.Currency.Update(currency);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return NotFound();
        var currency = await _unitOfWork.Currency.Get(t => t.Id == id);
        if (currency is null)
            return NotFound();
        _unitOfWork.Currency.Remove(currency);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Currency deleted successfully";
        return RedirectToAction(nameof(Index));
    }
}