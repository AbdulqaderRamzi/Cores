using System.Xml.Linq;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Accounting.Controllers;

[Area("Accounting")]
[Authorize(Roles = SD.ACCOUNTING_ROLE + "," + SD.ADMIN_ROLE)]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<IActionResult> Index()
    {
        var products = await _unitOfWork.Product.GetAll();
        return View(products);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        var product = new Product();
        if (id is not 0)
            product = await _unitOfWork.Product.Get(p => p.Id == id);
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(Product product)
    {
        if (!ModelState.IsValid)
            return View(product);
        if (product.Id is 0)
        {
            await _unitOfWork.Product.Add(product);
            TempData["success"] = "Product Created Successfully";
        }
        else
        {
            await _unitOfWork.Product.Update(product);
            TempData["success"] = "Product Updated Successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return NotFound();
        var product = await _unitOfWork.Product.Get(t => t.Id == id);
        if (product is null)
            return NotFound();
        _unitOfWork.Product.Remove(product);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Product deleted successfully";
        return RedirectToAction(nameof(Index));
    }
}