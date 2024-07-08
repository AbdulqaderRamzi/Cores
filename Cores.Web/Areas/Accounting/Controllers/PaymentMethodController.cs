using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Cores.Models.CRM;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Accounting.Controllers;

[Area("Accounting")]
[Authorize(Roles = SD.ACCOUNTING_ROLE + "," + SD.ADMIN_ROLE)]
public class PaymentMethodController : Controller
{
    
    private readonly IUnitOfWork _unitOfWork;

    public PaymentMethodController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var paymentMethods = await _unitOfWork.PaymentMethod.GetAll();
        return View(paymentMethods);
    }

    public async Task<IActionResult> Upsert(int? id)
    {
        if (id is null or 0)
            return View(new PaymentMethod());
        var paymentMethod = await _unitOfWork.PaymentMethod.Get(u => u.Id == id);
        return View(paymentMethod);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(PaymentMethod paymentMethod)
    {
        if (!ModelState.IsValid)
            return View(paymentMethod);
        if (paymentMethod.Id is 0)
            await _unitOfWork.PaymentMethod.Add(paymentMethod);
        else
            await _unitOfWork.PaymentMethod.Update(paymentMethod);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return NotFound();
        var paymentMethod = await _unitOfWork.PaymentMethod.Get(t => t.Id == id);
        if (paymentMethod is null)
            return NotFound();
        _unitOfWork.PaymentMethod.Remove(paymentMethod);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Payment Method deleted successfully";
        return RedirectToAction(nameof(Index));
    }
}