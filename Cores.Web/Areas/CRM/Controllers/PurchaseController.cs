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
[Authorize(Roles = SD.CRM_ROLE + "," + SD.ACCOUNTING_ROLE + "," + SD.ADMIN_ROLE)]
public class PurchaseController : Controller
{

    private readonly IUnitOfWork _unitOfWork;

    public PurchaseController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var purchases = await _unitOfWork.Purchase.GetAll(includeProperties: "Contact,Orders,PaymentMethod");
        return View(purchases);
    }

    public async Task<IActionResult> Upsert(int id, int contactId)
    {
        var purchaseVm = new PurchaseVm { Purchase = new Purchase() };

        await FillSelectionData(purchaseVm);

        if (contactId is not 0)
        {
            var contact = await _unitOfWork.Contact.Get(c => c.Id == contactId, isTracked: false) ?? new Contact();
            purchaseVm.ContactId = contact.Id;
            purchaseVm.Contact = contact;
        }
        
        if (id is 0) return View(purchaseVm);
        var purchase = await _unitOfWork.Purchase.Get(p => p.Id == id, isTracked: false, includeProperties: "Contact,Orders");
        if (purchase is null) return NotFound();
        /*purchaseVm.CustomerId = purchase.CustomerId;*/
        purchaseVm.Purchase = purchase;
        
        return View(purchaseVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(PurchaseVm purchaseVm)
    {
        Contact? contact;
        if (purchaseVm.ContactId is not null)
            contact = await _unitOfWork.Contact.Get(c => c.Id == purchaseVm.ContactId, isTracked: false, includeProperties:"Purchases");
        else if (purchaseVm.SelectedContactId is not null)
            contact = await _unitOfWork.Contact.Get(c => c.Id == purchaseVm.SelectedContactId, isTracked: false, includeProperties:"Purchases");
        else 
            contact = await _unitOfWork.Contact.Get(c => c.Id == purchaseVm.Purchase.ContactId, isTracked: false, includeProperties:"Purchases");
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
                    Quantity = order.Quantity,
                };
                newOrder.TotalPrice = newOrder.UnitPrice * newOrder.Quantity;
                totalPrice += newOrder.TotalPrice;
                await _unitOfWork.Order.Add(newOrder);
                purchaseVm.Purchase.Orders.Add(newOrder);
            }

        await _unitOfWork.SaveAsync();
        if (purchaseVm.Purchase.TaxId.HasValue)
        {
            var tax = await _unitOfWork.Tax.Get(t => t.Id == purchaseVm.Purchase.TaxId);
            if (tax is null)
                return NotFound();
            var taxRate = tax.Rate / 100;
            var taxAmount = totalPrice * taxRate;
            totalPrice += taxAmount; 
        }
        purchaseVm.Purchase.PurchaseAmount = totalPrice;
        purchaseVm.Purchase.ContactId = contact.Id;
        purchaseVm.Purchase.Contact = null; // reset the customer 
        
        bool isUpdate;
        if (purchaseVm.Purchase.Id == 0)
        {
            await _unitOfWork.Purchase.Add(purchaseVm.Purchase);
            TempData["success"] = "Purchase added successfully";
            isUpdate = false;
        }
        else
        {
            await _unitOfWork.Purchase.Update(purchaseVm.Purchase);
            TempData["success"] = "Purchase apdated successfully";
            isUpdate = true;
        }

        await _unitOfWork.SaveAsync();
        
        if (isUpdate)
        {
            var purchaseToDelete = contact.Purchases.FirstOrDefault(p => p.Id == purchaseVm.Purchase.Id);
            contact.Purchases.Remove(purchaseToDelete);
        }
        
        contact.Purchases.Add(purchaseVm.Purchase);
        await _unitOfWork.Contact.Update(contact, []);
        await _unitOfWork.SaveAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task FillSelectionData(PurchaseVm purchaseVm)
    {
        var products = await _unitOfWork.Product.GetAll();
        purchaseVm.Products = products.ToList();
            
        var contacts = await _unitOfWork.Contact.GetAll();
        var contactList = contacts.Select(x => new SelectListItem
        {
            Text = string.Concat(x.FirstName, " ", x.LastName),
            Value = x.Id.ToString()
        }).ToList();
        purchaseVm.Contacts = contactList;
            
        var paymentMethods = await _unitOfWork.PaymentMethod.GetAll();
        var paymentMethodList = paymentMethods.Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = x.Id.ToString()
        }).ToList();
        purchaseVm.PaymentMethods = paymentMethodList;

        var currencies = await _unitOfWork.Currency.GetAll();
        var currencyList = currencies.Select(x => new SelectListItem
        {
            Text = $"{x.Name} ({x.Code})",
            Value = x.Id.ToString()
        }).ToList();
        purchaseVm.Currencies = currencyList;
        
        var taxes = await _unitOfWork.Tax.GetAll();
        var taxList = taxes.Select(t => new SelectListItem
        {
            Text = $"{t.Name} ({t.Rate:F2})",
            Value = t.Id.ToString()
        }).ToList();
        purchaseVm.Taxes = taxList;
    }
    
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return NotFound();
        try
        {
            await _unitOfWork.Purchase.RemovePurchaseWithOrdersRaw(id);
            TempData["success"] = "Purchase deleted successfully";
        }
        catch (Exception ex)
        {
            // Log the exception
            TempData["error"] = "An error occurred while deleting the purchase and related orders";
        }

        return RedirectToAction(nameof(Index));
    }
    
    #region API CALL

    [HttpGet]
    public async Task<IActionResult> GetContactById(int id)
    {
        var contact = await _unitOfWork.Contact.Get(c => c.Id == id);
        return Json(contact);
    }


    #endregion
}