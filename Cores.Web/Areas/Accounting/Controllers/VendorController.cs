using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Accounting.Controllers;

[Area("Accounting")]
[Authorize(Roles = SD.ACCOUNTING_ROLE + "," + SD.ADMIN_ROLE)]
public class VendorController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public VendorController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IActionResult> Index()
    {
        var vendors = await _unitOfWork.Vendor.GetAll();
        return View(vendors);
    }
    
    public async Task<IActionResult> Upsert(int id)
    {
        var vendor = new Vendor();
        if (id is not 0)
            vendor = await _unitOfWork.Vendor.Get(p => p.Id == id);
        return View(vendor);
    }
    
    [HttpPost]
    public async Task<IActionResult> Upsert(Vendor vendor)
    {
        if (!ModelState.IsValid)
            return View(vendor);
        if (vendor.Id is 0)
        {
            await _unitOfWork.Vendor.Add(vendor);
            TempData["success"] = "Vendor Created Successfully";
        }
        else
        {
            await _unitOfWork.Vendor.Update(vendor);
            TempData["success"] = "Vendor Updated Successfully";
        }
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return NotFound();
        var vendor = await _unitOfWork.Vendor.Get(t => t.Id == id);
        if (vendor is null)
            return NotFound();
        _unitOfWork.Vendor.Remove(vendor);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Vendor deleted successfully";
        return RedirectToAction(nameof(Index));
    }
}