using Cores.DataService.Repository.IRepository;
using Cores.Models;
using Cores.Models.CRM;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Web.Areas.CRM.Controllers;

[Area("CRM")]
[Authorize(Roles = SD.CRM_ROLE + "," + SD.ADMIN_ROLE)]
public class CustomerController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CustomerController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        var customers = await _unitOfWork.Customer.GetAll();
        return View(customers);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        var tags = await _unitOfWork.Tag.GetAll();
        var tagsSelectItems = tags.Select(t => new SelectListItem
        {
            Text = t.Name,
            Value = t.Id.ToString()
        }).ToList();
        
        var languagesFromDb = await _unitOfWork.Language.GetAll();
        var languagesList = languagesFromDb.ToList();
        List<CheckBox> languages = [];
        for (var i = 1; i <= languagesList.Count; i++)
        {
            languages.Add(new CheckBox {Id = i, Value = languagesList[i-1].Value, isChecked = false});   
        }
       
        Customer? customer = new();
        if (id is not 0)
        {
            customer = await _unitOfWork.Customer.Get(u => u.Id == id, "Languages");
            if (customer is null)
                return RedirectToAction(nameof(Index));
            foreach (var lang in customer.Languages) 
            {
                    
                languages.First(l => string.Equals(l.Value,  lang.Value)).isChecked = true;
            }
        }
        var customerVm = new CustomerVM { Customer = customer, Tags = tagsSelectItems, LanguagesOptions = languages};
        return View(customerVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(IFormFile? file, List<string> languages, CustomerVM customerVm)
    {
        if (!ModelState.IsValid)
        {
            var tags = await _unitOfWork.Tag.GetAll();
            var tagsSelectItems = tags.Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = t.Id.ToString()
            }).ToList();
            customerVm.Tags = tagsSelectItems;
            return View(customerVm);
        }

        var wwwRootPath = _webHostEnvironment.WebRootPath;
        if (file is not null)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var customerPath = Path.Combine(wwwRootPath, @"images\customers");
            await using (var fileStream = new FileStream(Path.Combine(customerPath, fileName), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            if (!string.IsNullOrEmpty(customerVm.Customer.Document))
            {
                var oldImagePath = Path.Combine(wwwRootPath, customerVm.Customer.Document.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                    System.IO.File.Delete(oldImagePath);
            }
            customerVm.Customer.Document = @"\images\customers\" + fileName;
        } 
        
        customerVm.Customer.Email = customerVm.Customer.Email.ToLower().Trim();
        
            
        if (customerVm.Customer.Id is 0)
        {
            var languagesFromDb = await _unitOfWork.Language.GetAll();
            var languagesList = languagesFromDb.ToList();
            foreach (var lang in languages)
            {
                var language = languagesList.FirstOrDefault(l => l.Value == lang);
                if (language is null)
                {
                    language = new Language { Value = lang};
                    await _unitOfWork.Language.Add(language);
                }
                customerVm.Customer.Languages.Add(language);
            }
            await _unitOfWork.Customer.Add(customerVm.Customer);
            TempData["success"] = "Customer added successfully";
        }
        else
        {
            await _unitOfWork.Customer.Update(customerVm.Customer, languages);
            TempData["success"] = "Customer updated successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        var customer = await _unitOfWork.Customer.Get(c => c.Id == id);
        if (customer is null)
            return RedirectToAction(nameof(Index));
        if (!string.IsNullOrEmpty(customer.Document)){
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, customer.Document.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
                System.IO.File.Delete(oldImagePath);
        }
        _unitOfWork.Customer.Remove(customer);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Employee deleted successfully";
        return RedirectToAction(nameof(Index));
    }
}