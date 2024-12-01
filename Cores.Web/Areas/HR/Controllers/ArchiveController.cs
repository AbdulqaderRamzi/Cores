using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Web.Areas.HR.Controllers;

[Area("HR")]
[Authorize(Roles = SD.HR_ROLE + "," + SD.ADMIN_ROLE)]
public class ArchiveController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ArchiveController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        var archives = await _unitOfWork.Archive.GetAll(includeProperties: "Employee,ArchiveType");
        return View(archives);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        var archiveVm = new ArchiveVm
        {
            Archive = new(),
        };
        
        await FillSelectionDate(archiveVm);

        if (id == 0)
        {
            return View(archiveVm);
        }
        var archive = await _unitOfWork.Archive.Get(a => a.Id == id);
        if (archive is null)
            return NotFound();
        archiveVm.Archive = archive;
        return View(archiveVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(ArchiveVm archiveVm, IFormFile? file)
    {
        if (!ModelState.IsValid || file is null)
        {
            await FillSelectionDate(archiveVm);
            return View(archiveVm);
        }

        /* Start Of Handling The File */

        var wwwRootPath = _webHostEnvironment.WebRootPath;
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var documentsPath = Path.Combine(wwwRootPath, @"images\archives");
        await using (var fileStream = new FileStream(Path.Combine(documentsPath, fileName), FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        if (!string.IsNullOrEmpty(archiveVm.Archive.Path))
        {
            var oldDocumentPath = Path.Combine(wwwRootPath, archiveVm.Archive.Path.TrimStart('\\'));
            if (System.IO.File.Exists(oldDocumentPath))
            {
                System.IO.File.Delete(oldDocumentPath);
            }
        }
        archiveVm.Archive.Path = @"\images\archives\" + fileName;
        /*else
        {
            var archive = await _unitOfWork.Archive.Get(a => a.Id == archiveVm.Archive.Id, isTracked:false);
            if (archive is null)
                return NotFound();
            if (archiveVm.IsRemoved && !string.IsNullOrEmpty(archive.Path))
            {
                var oldDocumentPath = Path.Combine(_webHostEnvironment.WebRootPath, archive.Path.TrimStart('\\'));
                if (System.IO.File.Exists(oldDocumentPath))
                    System.IO.File.Delete(oldDocumentPath);
                archiveVm.Archive.Path = null;
            }
            else
            {
                archiveVm.Archive.Path = archive.Path;
            }
        }*/

        /* End Of Handling The File */
        if (archiveVm.Archive.Id == 0)
        {
            /*archiveVm.Archive.UploadDate = DateTime.Now;*/
            await _unitOfWork.Archive.Add(archiveVm.Archive);
            TempData["success"] = "Document added successfully";
        }
        else
        {
            await _unitOfWork.Archive.Update(archiveVm.Archive);
            TempData["success"] = "Document updated successfully";
        }
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> DownloadDocument(int id)
    {
        var archive = await _unitOfWork.Archive.Get(a => a.Id == id);
        if (archive == null || string.IsNullOrEmpty(archive.Path))
            return NotFound();

        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, archive.Path.TrimStart('\\'));
        if (!System.IO.File.Exists(filePath))
            return NotFound();

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(fileBytes, "application/octet-stream", Path.GetFileName(filePath));
    }

    public async Task<IActionResult> DeleteDocument(int? archiveId)
    {
        if (archiveId == null)
            return NotFound();
        var archive = await _unitOfWork.Archive.Get(a => a.Id == archiveId);
        if (archive == null)
            return NotFound();
        if (!string.IsNullOrEmpty(archive.Path))
        {
            var oldDocumentPath = Path.Combine(_webHostEnvironment.WebRootPath, archive.Path.TrimStart('\\'));
            if (System.IO.File.Exists(oldDocumentPath))
                System.IO.File.Delete(oldDocumentPath);
        }
        else
        {
            return RedirectToAction(nameof(Upsert), new { id = archiveId });
        }
        archive.Path = null;
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Upsert), new { id = archiveId });
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (id == 0)
            return BadRequest();
        var archive = await _unitOfWork.Archive.Get(a => a.Id == id);
        if (archive is null)
            return NotFound();
        if (!string.IsNullOrEmpty(archive.Path))
        {
            var oldDocumentPath = Path.Combine(_webHostEnvironment.WebRootPath, archive.Path.TrimStart('\\'));
            if (System.IO.File.Exists(oldDocumentPath))
                System.IO.File.Delete(oldDocumentPath);
        }

        _unitOfWork.Archive.Remove(archive);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }


    private async Task FillSelectionDate(ArchiveVm archiveVm)
    {
        var employees = await _unitOfWork.ApplicationUser.GetAll();
        archiveVm.Employees = employees.Select(e => new SelectListItem
        {
            Text = $"{e.FirstName} {e.LastName}",
            Value = e.Id
        }).ToList();
        
        var archiveTypes = await _unitOfWork.ArchiveType.GetAll();
        archiveVm.ArchiveTypes = archiveTypes.Select(e => new SelectListItem
        {
            Text = e.Name,
            Value = e.Id.ToString()
        }).ToList();   
    }
}