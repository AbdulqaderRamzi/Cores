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
        var purchases = await _unitOfWork.Purchase.GetAll(includeProperties: "Customer,Orders");
        return View(purchases);
    }

    public async Task<IActionResult> Upsert(int id, int customerId)
    {
            var purchaseVm = new PurchaseVm { Purchase = new Purchase() };

        var products = await _unitOfWork.Product.GetAll();
        purchaseVm.Products = products.ToList();

        if (customerId is not 0)
        {
            var customer = await _unitOfWork.Customer.Get(c => c.Id == customerId, isTracked: false) ?? new Customer();
            purchaseVm.CustomerId = customer.Id;
            purchaseVm.Customer = customer;
        }
        var customers = await _unitOfWork.Customer.GetAll();
        var customerList = customers.Select(x => new SelectListItem
        {
            Text = string.Concat(x.FirstName, " ", x.LastName),
            Value = x.Id.ToString()
        }).ToList();
        purchaseVm.Customers = customerList;

        if (id is 0) return View(purchaseVm);
        var purchase = await _unitOfWork.Purchase.Get(p => p.Id == id, isTracked: false, includeProperties: "Customer,Orders");
        if (purchase is null) return NotFound();
        /*purchaseVm.CustomerId = purchase.CustomerId;*/
        purchaseVm.Purchase = purchase;
        
        return View(purchaseVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(PurchaseVm purchaseVm)
    {
        Customer? customer;
        if (purchaseVm.CustomerId is not null)
            customer = await _unitOfWork.Customer.Get(c => c.Id == purchaseVm.CustomerId, isTracked: false, includeProperties:"Purchases");
        else if (purchaseVm.SelectedCustomerId is not null)
            customer = await _unitOfWork.Customer.Get(c => c.Id == purchaseVm.SelectedCustomerId, isTracked: false, includeProperties:"Purchases");
        else 
            customer = await _unitOfWork.Customer.Get(c => c.Id == purchaseVm.Purchase.CustomerId, isTracked: false, includeProperties:"Purchases");
        purchaseVm.Purchase.Orders.Clear();
        var orders = JsonConvert.DeserializeObject<List<Order>>(purchaseVm.SerializedProducts);
        decimal totalPrice = 0;

        if (orders is not null)
            foreach (var order in orders)
            {
                var newOrder = new Order
                {
                    Name = order.Name,
                    UnitPrice = order.UnitPrice,
                    Quantity = order.Quantity
                };
                
                newOrder.TotalPrice = newOrder.UnitPrice * newOrder.Quantity;
                totalPrice += newOrder.TotalPrice;
                await _unitOfWork.Order.Add(newOrder);
                purchaseVm.Purchase.Orders.Add(newOrder);
            }

        await _unitOfWork.SaveAsync();
        purchaseVm.Purchase.PurchaseAmount = totalPrice;
        purchaseVm.Purchase.CustomerId = customer.Id;
        purchaseVm.Purchase.Customer = null; // reset the customer 


        bool isUpdate;
        if (purchaseVm.Purchase.Id == 0)
        {
            await _unitOfWork.Purchase.Add(purchaseVm.Purchase);
            isUpdate = false;
        }
        else
        {
            await _unitOfWork.Purchase.Update(purchaseVm.Purchase);
            isUpdate = true;
        }

        await _unitOfWork.SaveAsync();
        
        if (isUpdate)
        {
            var purchaseToDelete = customer.Purchases.FirstOrDefault(p => p.Id == purchaseVm.Purchase.Id);
            customer.Purchases.Remove(purchaseToDelete);
        }
        
        customer.Purchases.Add(purchaseVm.Purchase);
        await _unitOfWork.Customer.Update(customer, []);
        await _unitOfWork.SaveAsync();

        return RedirectToAction(nameof(Index));
    }


    #region API CALL

    [HttpGet]
    public async Task<IActionResult> GetCustomerById(int id)
    {
        var customer = await _unitOfWork.Customer.Get(c => c.Id == id);
        return Json(customer);
    }


    #endregion
}