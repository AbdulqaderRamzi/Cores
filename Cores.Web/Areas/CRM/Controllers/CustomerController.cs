using System.Text;
using Cores.DataService.Repository.IRepository;
using Cores.Models;
using Cores.Models.CRM;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
        List<CheckBox> languagesOptions = [];
        for (var i = 1; i <= languagesList.Count; i++)
            languagesOptions.Add(new CheckBox { Id = i, Value = languagesList[i - 1].Value, isChecked = false });

        Customer? customer = new();
        List<Purchase> purchasesList = [];
        var selectedTagIds = new List<int>();
        if (id is not 0)
        {
            customer = await _unitOfWork.Customer.Get(u => u.Id == id, "Languages,Tags");
            if (customer is null)
                return RedirectToAction(nameof(Index)); 
            foreach (var lang in customer.Languages)
                languagesOptions.First(l => string.Equals(l.Value, lang.Value)).isChecked = true;
            selectedTagIds = customer.Tags.Select(t => t.Id).ToList();
            var purchases = await _unitOfWork.Purchase.GetAll(p => p.CustomerId == customer.Id);
            purchasesList.AddRange(purchases);
        }
        
        var customerVm = new CustomerVm
        {
            Customer = customer,
            Tags = tagsSelectItems,
            LanguagesOptions = languagesOptions, 
            SelectedTagIds = selectedTagIds, 
            Purchases = purchasesList
        };
        return View(customerVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(IFormFile? file, List<string> languages, CustomerVm customerVm,
        bool createPurchase = false)
    {
        if (!ModelState.IsValid)
        {
            var tags = await _unitOfWork.Tag.GetAll();
            customerVm.Tags = tags.Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = t.Id.ToString()
            }).ToList();
            var languagesFromDb = await _unitOfWork.Language.GetAll();
            var languagesList = languagesFromDb.ToList();
            List<CheckBox> languagesOptions = [];
            for (var i = 1; i <= languagesList.Count; i++)
                languagesOptions.Add(new CheckBox { Id = i, Value = languagesList[i - 1].Value, isChecked = false });

            customerVm.LanguagesOptions = languagesOptions;
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
        else
        {
            var customerFromDb = await _unitOfWork.Customer.Get(c => c.Id == customerVm.Customer.Id, isTracked: false);
            if (customerFromDb?.Document is not null)
                customerVm.Customer.Document = customerFromDb.Document;
        }

        customerVm.Customer.Email = customerVm.Customer.Email.ToLower().Trim();
        
        if (customerVm.SelectedTagIds is not null)
        {
            var selectedTags = await _unitOfWork.Tag.GetAll(t => customerVm.SelectedTagIds.Contains(t.Id));
            customerVm.Customer.Tags = selectedTags.ToList();
        }

        if (customerVm.Customer.Id is 0)
        {
            var languagesFromDb = await _unitOfWork.Language.GetAll();
            var languagesList = languagesFromDb.ToList();
            foreach (var lang in languages)
            {
                var language = languagesList.FirstOrDefault(l => l.Value == lang);
                if (language is null)
                {
                    language = new Language { Value = lang };
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
        return createPurchase ? RedirectToAction(nameof(Upsert), "Purchase", new { customerId = customerVm.Customer.Id}) 
                              : RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return NotFound();
        var customer = await _unitOfWork.Customer.Get(c => c.Id == id);
        if (customer is null)
            return NotFound();
        if (!string.IsNullOrEmpty(customer.Document))
        {
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, customer.Document.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
                System.IO.File.Delete(oldImagePath);
        }

        _unitOfWork.Customer.Remove(customer);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Customer deleted successfully";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> DownloadDocument(string fileName)
    {
        var filePath = _webHostEnvironment.WebRootPath + fileName;

        if (!System.IO.File.Exists(filePath)) return NotFound();

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(fileBytes, "application/octet-stream", fileName);
    }

    public async Task<IActionResult> DeleteDocument(int? customerId)
    {
        if (customerId == null)
            return NotFound();
        var customer = await _unitOfWork.Customer.Get(c => c.Id == customerId);
        if (customer == null)
            return NotFound();
        if (!string.IsNullOrEmpty(customer.Document))
        {
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, customer.Document.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
                System.IO.File.Delete(oldImagePath);
        }
        else
        {
            return RedirectToAction(nameof(Upsert), new { id = customerId });
        }

        customer.Document = null;
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Upsert), new { id = customerId });
    }
    
    public async Task<IActionResult> DownloadSummery(int? id)
    {
        if (id is null)
            return NotFound();
        var customer = await _unitOfWork.Customer.Get(c => c.Id == id, includeProperties: "Languages");
        if (customer is null)
            return NotFound();
        var wwwRootPath = _webHostEnvironment.WebRootPath;
        var imagePath = Path.Combine(wwwRootPath, @"img/login-image.jpg");
        var imageData = await System.IO.File.ReadAllBytesAsync(imagePath);

        var data = Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Margin(20);
                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        var datetime = DateTime.Now;
                        col.Item().Padding(5).AlignLeft().Text($"Issued date: {datetime.Date}").FontSize(15);
                    });
                    row.ConstantItem(140).Height(100).Image(imageData);
                });
                page.Content().Column(col1 =>
                {
                    col1.Item().Text("Personal Info").Bold().FontSize(25).Underline();
                    col1.Item().PaddingTop(30).Row(row =>
                    {
                        row.RelativeItem().Text(txt =>
                        {
                            txt.Span("Full Name: ").Bold().FontSize(15);
                            txt.Span(string.Concat(customer.FirstName, " ", customer.LastName)).FontSize(15);
                        });
                        row.RelativeItem().Text(txt =>
                        {
                            txt.Span("Email: ").Bold().FontSize(15);
                            txt.Span(customer.Email).FontSize(15);
                        });
                    });
                    col1.Item().PaddingTop(15).Row(row =>
                    {
                        row.RelativeItem().Text(txt =>
                        {
                            txt.Span("Phone Number: ").Bold().FontSize(15);
                            txt.Span(customer.PhoneNumber).FontSize(15);
                        });
                        row.RelativeItem().Text(txt =>
                        {
                            txt.Span("Gender: ").Bold().FontSize(15);
                            txt.Span(customer.Gender).FontSize(15);
                        });
                    });
                    col1.Item().PaddingTop(15).Row(row =>
                    {
                        row.RelativeItem().Text(txt =>
                        {
                            txt.Span("Nationality: ").Bold().FontSize(15);
                            txt.Span("Palestine").FontSize(15);
                        });
                        row.RelativeItem().Text(txt =>
                        {
                            txt.Span("Country: ").Bold().FontSize(15);
                            txt.Span("Kuwait").FontSize(15);
                        });
                    });
                    col1.Item().PaddingTop(15).Row(row =>
                    {
                        row.RelativeItem().Text(txt =>
                        {
                            txt.Span("City: ").Bold().FontSize(15);
                            txt.Span(customer.City).FontSize(15);
                        });
                        row.RelativeItem().Text(txt =>
                        {
                            txt.Span("State: ").Bold().FontSize(15);
                            txt.Span(customer.State).FontSize(15);
                        });
                    });
                    col1.Item().PaddingTop(15).Row(row =>
                    {
                        row.RelativeItem().Text(txt =>
                        {
                            txt.Span("BLock: ").Bold().FontSize(15);
                            txt.Span("11").FontSize(15);
                        });
                        row.RelativeItem().Text(txt =>
                        {
                            txt.Span("Street: ").Bold().FontSize(15);
                            txt.Span(customer.StreetAddress).FontSize(15);
                        });
                    });
                    col1.Item().PaddingTop(15).Row(row =>
                    {
                        row.RelativeItem().Text(txt =>
                        {
                            txt.Span("Languages: ").Bold().FontSize(15);
                            List<string> languages = [];
                            languages.AddRange(customer.Languages.Select(lang => lang.Value));
                            txt.Span(string.Join(", ", languages)).FontSize(15);
                        });
                    });
                    col1.Item().PaddingTop(30).Text("Purchase Info").Bold().FontSize(25).Underline();
                });
                page.Footer().AlignRight().Text(txt =>
                {
                    txt.Span("Page ").FontSize(10);
                    txt.CurrentPageNumber().FontSize(10);
                    txt.Span(" of ").FontSize(10);
                    txt.TotalPages().FontSize(10);
                });
            });
        }).GeneratePdf();

        Stream stream = new MemoryStream(data);
        return File(stream, "application/pdf", "Mahmoud.pdf");
    }
}