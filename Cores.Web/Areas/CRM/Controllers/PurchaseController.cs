using System.Xml.Linq;
using Cores.DataService.Repository.IRepository;
using Cores.Models.CRM;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Cores.Web.Areas.CRM.Controllers;

[Area("CRM")]
[Authorize(Roles = SD.CRM_ROLE + "," + SD.ADMIN_ROLE)]
public class PurchaseController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public PurchaseController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var purchases = await _unitOfWork.Purchase.GetAll();
        return View(purchases);
    }

    public async Task<IActionResult> Upsert(int id, int customerId)
    {
        var purchaseVm = new PurchaseVM {Purchase = new()};

        var products = await _unitOfWork.Product.GetAll();
        purchaseVm.Products = products.ToList();
        
        if (customerId is not 0)
        {   
            var customer = await _unitOfWork.Customer.Get(c => c.Id == customerId, isTracked: false) ?? new Customer();
            purchaseVm.CustomerName = string.Concat(customer.FirstName, " ", customer.LastName);
            purchaseVm.CustomerId = customer.Id;
        }
        else
        {
            var customers = await _unitOfWork.Customer.GetAll();
            var customerList = customers.Select(x => new SelectListItem
            {
                Text = string.Concat(x.FirstName, " ", x.LastName),
                Value = x.Id.ToString()
            }).ToList();
            purchaseVm.Customers = customerList;
        }
        
        if (id is 0) return View(purchaseVm);
        var purchase = await _unitOfWork.Purchase.Get(p => p.Id == id, isTracked: false);
        if (purchase is null) return NotFound();
        purchaseVm.Purchase = purchase;
        return View(purchaseVm);
    }

    [HttpPost]  
    public async Task<IActionResult> Upsert(PurchaseVM purchaseVm)
    {
        Customer? customer;
        if (purchaseVm.CustomerId is not null)
        {
            customer = await _unitOfWork.Customer.Get(c => c.Id == purchaseVm.CustomerId, isTracked: false);
        }
        else
        {
            customer = await _unitOfWork.Customer.Get(c => c.Id == purchaseVm.SelectedCustomerId, isTracked: false);
        } 
        
        if (customer is null)
            return NotFound();
        
        var orders = JsonConvert.DeserializeObject<List<Order>>(purchaseVm.SerializedProducts);
        var totalPrice = 0m;
        if (orders is not null)
        {
            foreach (var order in orders)
            {
                var newOrder = new Order
                {
                    Name = order.Name,
                    UnitPrice = order.UnitPrice, 
                    Quantity = order.Quantity 
                };
                await _unitOfWork.Order.Add(newOrder);
                purchaseVm.Purchase.Orders.Add(newOrder);
            }
        }

        await _unitOfWork.SaveAsync();
        purchaseVm.Purchase.PurchaseAmount = totalPrice;
        purchaseVm.Purchase.CustomerId = customer.Id;

        if (purchaseVm.Purchase.Id == 0)
        {
            await _unitOfWork.Purchase.Add(purchaseVm.Purchase);
        }
        else
        {
            var existingPurchase = await _unitOfWork.Purchase.Get(p => p.Id == purchaseVm.Purchase.Id);
            if (existingPurchase is null)
                return View();

            existingPurchase.Orders = purchaseVm.Purchase.Orders;
            _unitOfWork.Purchase.Update(existingPurchase);
        }

        await _unitOfWork.SaveAsync();

        customer.Purchases.Add(purchaseVm.Purchase);
        await _unitOfWork.Customer.Update(customer, []);
        await _unitOfWork.SaveAsync();

        return RedirectToAction(nameof(Index));
    }
}
