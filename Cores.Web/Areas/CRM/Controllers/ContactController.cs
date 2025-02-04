﻿using Cores.DataService.Repository.IRepository;
using Cores.Models;
using Cores.Models.CRM;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuestPDF.Fluent;


namespace Cores.Web.Areas.CRM.Controllers;

[Area("CRM")]
[Authorize(Roles = SD.CRM_ROLE + "," + SD.ACCOUNTING_ROLE + "," + SD.ADMIN_ROLE)]
public class ContactController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ContactController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index(bool? isCustomer = null)
    {
        var contacts =
            isCustomer is null ? await _unitOfWork.Contact.GetAll()
            : (bool) isCustomer ? await _unitOfWork.Contact.GetAll(c => c.Status == "Customer")
            : await _unitOfWork.Contact.GetAll(c => c.Status == "Lead");
        ViewBag.isCustomer = isCustomer;
        return View(contacts);
    }
    
    
    [Authorize(Roles = SD.CRM_ROLE + "," + SD.ADMIN_ROLE)]
    public async Task<IActionResult> Upsert(int id, bool? isItCustomer)
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

        Contact? contact = new();
        List<Purchase> purchasesList = [];
        var selectedTagIds = new List<int>();
        if (id is not 0)
        {
            contact = await _unitOfWork.Contact.GetEventWithRelatedData(id);
            if (contact is null)
                return RedirectToAction(nameof(Index)); 
            foreach (var lang in contact.Languages)
                languagesOptions.First(l => string.Equals(l.Value, lang.Value)).isChecked = true;
            selectedTagIds = contact.Tags.Select(t => t.Id).ToList();
            var purchases = await _unitOfWork.Purchase.GetAll(p => p.ContactId == contact.Id);
            purchasesList.AddRange(purchases);
        }
        
        
        var contactVm = new ContactVm
        {
            Contact = contact,
            Tags = tagsSelectItems,
            LanguagesOptions = languagesOptions, 
            SelectedTagIds = selectedTagIds, 
            Purchases = purchasesList
        };
        ViewBag.isCustomer = isItCustomer;
        return View(contactVm);
    }

    [HttpPost]
    [Authorize(Roles = SD.CRM_ROLE + "," + SD.ADMIN_ROLE)]
    public async Task<IActionResult> Upsert(IFormFile? file, List<string> languages,
        ContactVm contactVm, bool createPurchase, bool? isItCustomer)
    {
        if (!ModelState.IsValid)
        {
            var tags = await _unitOfWork.Tag.GetAll();
            contactVm.Tags = tags.Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = t.Id.ToString()
            }).ToList();
            var languagesFromDb = await _unitOfWork.Language.GetAll();
            var languagesList = languagesFromDb.ToList();
            List<CheckBox> languagesOptions = [];
            for (var i = 1; i <= languagesList.Count; i++)
                languagesOptions.Add(new CheckBox { Id = i, Value = languagesList[i - 1].Value, isChecked = false });

            contactVm.LanguagesOptions = languagesOptions;
            
            ViewBag.isCustomer = isItCustomer;
            return View(contactVm);
        }

        var wwwRootPath = _webHostEnvironment.WebRootPath;
        if (file is not null)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var contactPath = Path.Combine(wwwRootPath, @"images\contacts");
            await using (var fileStream = new FileStream(Path.Combine(contactPath, fileName), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            if (!string.IsNullOrEmpty(contactVm.Contact.Document))
            {
                var oldImagePath = Path.Combine(wwwRootPath, contactVm.Contact.Document.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                    System.IO.File.Delete(oldImagePath);
            }

            contactVm.Contact.Document = @"\images\contacts\" + fileName;
        }
        else
        {
            var contactFromDb = await _unitOfWork.Contact.Get(c => c.Id == contactVm.Contact.Id, isTracked: false);
            if (contactFromDb?.Document is not null)
                contactVm.Contact.Document = contactFromDb.Document;
        }

        contactVm.Contact.Email = contactVm.Contact.Email.ToLower().Trim();
        
        if (contactVm.SelectedTagIds is not null)
        {
            var selectedTags = await _unitOfWork.Tag.GetAll(t => contactVm.SelectedTagIds.Contains(t.Id));
            contactVm.Contact.Tags = selectedTags.ToList();
        }

        if (contactVm.Contact.Id is 0)
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

                contactVm.Contact.Languages.Add(language);
            }
            
            await _unitOfWork.Contact.Add(contactVm.Contact);
            await _unitOfWork.SaveAsync();
            TempData["success"] = "Contact added successfully";
            return createPurchase ? RedirectToAction(nameof(Upsert), "Purchase", new { contactId = contactVm.Contact.Id }) :
                         RedirectToAction(nameof(Index), new { isCustomer = isItCustomer });
        }
        await _unitOfWork.Contact.Update(contactVm.Contact, languages);
        await _unitOfWork.SaveAsync();
        if (createPurchase)
        {
            return RedirectToAction(nameof(Upsert), "Purchase", new { contactId = contactVm.Contact.Id });
        }
        TempData["success"] = "Contact updated successfully";
        return RedirectToAction(nameof(Index), new { isCustomer = isItCustomer });
    }

    [Authorize(Roles = SD.CRM_ROLE + "," + SD.ADMIN_ROLE)]
    public async Task<IActionResult> Delete(int? id, bool isItCustomer)
    {
        if (id is null)
            return NotFound();
        var contact = await _unitOfWork.Contact.Get(c => c.Id == id);
        if (contact is null)
            return NotFound();
        if (!string.IsNullOrEmpty(contact.Document))
        {
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, contact.Document.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
                System.IO.File.Delete(oldImagePath);
        }

        _unitOfWork.Contact.Remove(contact);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Contact deleted successfully";
        return RedirectToAction(nameof(Index), new { isCustomer = isItCustomer });
    }

    [Authorize(Roles = SD.CRM_ROLE + "," + SD.ADMIN_ROLE)]
    public async Task<IActionResult> DownloadDocument(string fileName)
    {
        var filePath = _webHostEnvironment.WebRootPath + fileName;

        if (!System.IO.File.Exists(filePath)) return NotFound();

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(fileBytes, "application/octet-stream", fileName);
    }

    [Authorize(Roles = SD.CRM_ROLE + "," + SD.ADMIN_ROLE)]
    public async Task<IActionResult> DeleteDocument(int? contactId)
    {
        if (contactId is null)
            return NotFound();
            
        if (contactId is 0)
        {
            return RedirectToAction(nameof(Upsert));
        }
            
        var contact = await _unitOfWork.Contact.Get(c => c.Id == contactId);
        if (contact == null)
            return NotFound();
        if (!string.IsNullOrEmpty(contact.Document))
        {
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, contact.Document.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
                System.IO.File.Delete(oldImagePath);
        }
        else
        {
            return RedirectToAction(nameof(Upsert), new { id = contactId });
        }

        contact.Document = null;
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Upsert), new { id = contactId });
    }
    
    public async Task<IActionResult> DownloadSummery(int? id)
    {
        if (id is null)
            return NotFound();
        var contact = await _unitOfWork.Contact.Get(c => c.Id == id, 
            includeProperties: "Languages,Purchases.Orders,Purchases.Currency,Purchases.PaymentMethod,Purchases.Tax");
        if (contact is null)
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
                    // Personal Info Section
                    col1.Item().Text("Personal Info").Bold().FontSize(25).Underline();
                    col1.Item().PaddingTop(30).Row(row =>
                    {
                        row.RelativeItem().Text(txt =>
                        {
                            txt.Span("Full Name: ").Bold().FontSize(15);
                            txt.Span(string.Concat(contact.FirstName, " ", contact.LastName)).FontSize(15);
                        });
                        row.RelativeItem().Text(txt =>
                        {
                            txt.Span("Email: ").Bold().FontSize(15);
                            txt.Span(contact.Email).FontSize(15);
                        });
                    });
                    col1.Item().PaddingTop(15).Row(row =>
                    {
                        row.RelativeItem().Text(txt =>
                        {
                            txt.Span("Phone Number: ").Bold().FontSize(15);
                            txt.Span(contact.PhoneNumber).FontSize(15);
                        });
                        row.RelativeItem().Text(txt =>
                        {
                            txt.Span("Gender: ").Bold().FontSize(15);
                            txt.Span(contact.Gender).FontSize(15);
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
                            txt.Span(contact.City).FontSize(15);
                        });
                        row.RelativeItem().Text(txt =>
                        {
                            txt.Span("State: ").Bold().FontSize(15);
                            txt.Span(contact.State).FontSize(15);
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
                            txt.Span(contact.StreetAddress).FontSize(15);
                        });
                    });
                    col1.Item().PaddingTop(15).Row(row =>
                    {
                        row.RelativeItem().Text(txt =>
                        {
                            txt.Span("Languages: ").Bold().FontSize(15);
                            List<string> languages = [];
                            languages.AddRange(contact.Languages.Select(lang => lang.Value));
                            txt.Span(string.Join(", ", languages)).FontSize(15);
                        });
                    });

                    // Purchase Info Section
                    col1.Item().PaddingTop(30).Text("Purchase History").Bold().FontSize(25).Underline();
                    
                    if (!contact.Purchases.Any())
                    {
                        col1.Item().PaddingTop(10).Text("No purchase history available.").FontSize(15);
                    }
                    else
                    {
                        foreach (var purchase in contact.Purchases.OrderByDescending(p => p.PurchaseDate))
                        {
                            col1.Item().PaddingTop(20).Column(purchaseCol =>
                            {
                                // Purchase Header
                                purchaseCol.Item().Row(row =>
                                {
                                    row.RelativeItem().Text(txt =>
                                    {
                                        txt.Span("Purchase Date: ").Bold().FontSize(15);
                                        txt.Span(purchase.PurchaseDate.ToString("dd/MM/yyyy")).FontSize(15);
                                    });
                                    row.RelativeItem().Text(txt =>
                                    {
                                        txt.Span("Status: ").Bold().FontSize(15);
                                        txt.Span(purchase.Status).FontSize(15);
                                    });
                                });

                                // Purchase Details
                                purchaseCol.Item().PaddingTop(10).Row(row =>
                                {
                                    row.RelativeItem().Text(txt =>
                                    {
                                        txt.Span("Payment Method: ").Bold().FontSize(15);
                                        txt.Span(purchase.PaymentMethod?.Name ?? "N/A").FontSize(15);
                                    });
                                    row.RelativeItem().Text(txt =>
                                    {
                                        txt.Span("Currency: ").Bold().FontSize(15);
                                        txt.Span(purchase.Currency?.Code ?? "N/A").FontSize(15);
                                    });
                                });

                                // Orders Table
                                if (purchase.Orders.Any())
                                {
                                    purchaseCol.Item().PaddingTop(10).Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(3);
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                        });

                                        // Table Header
                                        table.Header(header =>
                                        {
                                            header.Cell().Background("#D1D5DB").Padding(5).Text("Item").Bold();
                                            header.Cell().Background("#D1D5DB").Padding(5).Text("Quantity").Bold();
                                            header.Cell().Background("#D1D5DB").Padding(5).Text("Unit Price").Bold();
                                            header.Cell().Background("#D1D5DB").Padding(5).Text("Total").Bold();
                                        });

                                        // Table Content
                                        foreach (var order in purchase.Orders)
                                        {
                                            table.Cell().Padding(5).Text(order.Name);
                                            table.Cell().Padding(5).Text(order.Quantity.ToString());
                                            table.Cell().Padding(5).Text(order.UnitPrice.ToString("C"));
                                            table.Cell().Padding(5).Text(order.TotalPrice.ToString("C"));
                                        }
                                    });

                                    // Purchase Summary
                                    purchaseCol.Item().PaddingTop(10).Row(row =>
                                    {
                                        row.RelativeItem().AlignRight().Text(txt =>
                                        {
                                            txt.Span("Total Amount: ").Bold().FontSize(15);
                                            txt.Span($"{purchase.PurchaseAmount:C} {purchase.Currency?.Code}").FontSize(15);
                                        });
                                    });

                                    if (!string.IsNullOrEmpty(purchase.Note))
                                    {
                                        purchaseCol.Item().PaddingTop(5).Text(txt =>
                                        {
                                            txt.Span("Note: ").Bold().FontSize(13);
                                            txt.Span(purchase.Note).FontSize(13);
                                        });
                                    }
                                }
                            });
                        }
                    }
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
        return File(stream, "application/pdf", $"{contact.FirstName}_{contact.LastName}_Summary.pdf");
    }
}