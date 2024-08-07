﻿using Cores.DataService.Repository.IRepository;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Web.Areas.HR.Controllers;

[Area("HR")]
[Authorize(Roles = SD.HR_ROLE + "," + SD.ADMIN_ROLE)]
public class JobApplicationController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public JobApplicationController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        var jobApplications = await _unitOfWork.JobApplication.GetAll
            (includeProperties:"Recruitment");
        return View(jobApplications);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        var recruitments = await _unitOfWork.Recruitment.GetAll();
        var recruitmentsList = recruitments.Select(r => new SelectListItem
        {
            Text = r.JobTitle,
            Value = r.Id.ToString()
        }).ToList();
        
        var jobApplicationVm = new JobApplicationVm()
        { 
            JobApplication = new(),
            Recruitments = recruitmentsList
        };
    
        if (id is 0)
        {
            return View(jobApplicationVm);
        }

        var jobApplication = await _unitOfWork.JobApplication.Get(ja => ja.Id == id);
        if (jobApplication is null)
            return NotFound();
        jobApplicationVm.JobApplication = jobApplication;
        return View(jobApplicationVm);
    }
    
    [HttpPost]
    public async Task<IActionResult> Upsert(JobApplicationVm jobApplicationVm, IFormFile? file)
    {
        if (!ModelState.IsValid)
        {
            var recruitments = await _unitOfWork.Recruitment.GetAll();
            var recruitmentsList = recruitments.Select(r => new SelectListItem
            {
                Text = r.JobTitle,
                Value = r.Id.ToString()
            }).ToList();
            jobApplicationVm.Recruitments = recruitmentsList;
            return View(jobApplicationVm);
            
        }

        /* Start Of Handling The File */

        var wwwRootPath = _webHostEnvironment.WebRootPath;
        if (file is not null)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var applicantsPath = Path.Combine(wwwRootPath, @"images\applicants");
            await using (var fileStream = new FileStream(Path.Combine(applicantsPath, fileName), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            if (!string.IsNullOrEmpty(jobApplicationVm.JobApplication.Resume))
            {
                var oldResumePath = Path.Combine(wwwRootPath, jobApplicationVm.JobApplication.Resume.TrimStart('\\'));
                if (System.IO.File.Exists(oldResumePath))
                {
                    System.IO.File.Delete(oldResumePath);
                }
            }
            jobApplicationVm.JobApplication.Resume = @"\images\applicants\" + fileName;
        }
        else
        {
            var jobApplication = await _unitOfWork.JobApplication.Get(c => c.Id == jobApplicationVm.JobApplication.Id, isTracked: false);
            if (jobApplication?.Resume is not null)
                jobApplicationVm.JobApplication.Resume = jobApplication.Resume;
        }

        /* End Of Handling The File */

        if (jobApplicationVm.JobApplication.Id is 0)
        {
            await _unitOfWork.JobApplication.Add(jobApplicationVm.JobApplication);
            TempData["success"] = "JobApplication added successfully";
        }
        else
        {
            await _unitOfWork.JobApplication.Update(jobApplicationVm.JobApplication);
            TempData["success"] = "JobApplication updated successfully";
        }
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> DownloadDResume(string fileName)
    {
        var filePath = _webHostEnvironment.WebRootPath + fileName;

        if (!System.IO.File.Exists(filePath)) return NotFound();

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(fileBytes, "application/octet-stream", fileName);
    }
    
    public async Task<IActionResult> DeleteResume(int? jobApplicationId)
    {
        if (jobApplicationId == null)
            return NotFound();
        var jobApplication = await _unitOfWork.JobApplication.Get(c => c.Id == jobApplicationId);
        if (jobApplication == null)
            return NotFound();
        if (!string.IsNullOrEmpty(jobApplication.Resume))
        {
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, jobApplication.Resume.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
                System.IO.File.Delete(oldImagePath);
        }
        else
        {
            return RedirectToAction(nameof(Upsert), new { id = jobApplicationId });
        }

        jobApplication.Resume = null;
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Upsert), new { id = jobApplicationId });
    }


    public async Task<IActionResult> Delete(int id)
    {
        if (id is 0)
            return BadRequest();
        var jobApplication = await _unitOfWork.JobApplication.Get(ja => ja.Id == id);
        if (jobApplication is null)
            return NotFound();
        if (!string.IsNullOrEmpty(jobApplication.Resume))
        {
            var oldResumePath = Path.Combine(_webHostEnvironment.WebRootPath, jobApplication.Resume.TrimStart('\\'));
            if (System.IO.File.Exists(oldResumePath))
                System.IO.File.Delete(oldResumePath);
        }
            
        _unitOfWork.JobApplication.Remove(jobApplication);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
    
}



/*
using Cores.DataService.Repository.IRepository;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Web.Areas.HR.Controllers;

[Area("HR")]
[Authorize(Roles = SD.HR_ROLE + "," + SD.ADMIN_ROLE)]
public class JobApplicationController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public JobApplicationController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        var jobApplications = await _unitOfWork.JobApplication.GetAll(includeProperties: "Recruitment");
        return View(jobApplications);
    }

    public async Task<IActionResult> Upsert(int? id)
    {
        var jobApplicationVm = new JobApplicationVm()
        {
            JobApplication = new(),
            Recruitments = await GetRecruitmentsList()
        };

        if (id == null || id == 0)
        {
            return View(jobApplicationVm);
        }

        jobApplicationVm.JobApplication = await _unitOfWork.JobApplication.Get(ja => ja.Id == id);
        if (jobApplicationVm.JobApplication == null)
        {
            return NotFound();
        }

        return View(jobApplicationVm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upsert(JobApplicationVm jobApplicationVm)
    {
        if (!ModelState.IsValid)
        {
            jobApplicationVm.Recruitments = await GetRecruitmentsList();
            return View(jobApplicationVm);
        }

        if (jobApplicationVm.ResumeFile is not null)
        {
            string fileName = await SaveResumeFile(jobApplicationVm.ResumeFile);
            if (!string.IsNullOrEmpty(jobApplicationVm.JobApplication.Resume))
            {
                DeleteOldResumeFile(jobApplicationVm.JobApplication.Resume);
            }
            jobApplicationVm.JobApplication.Resume = fileName;
        }

        if (jobApplicationVm.JobApplication.Id == 0)
        {
            await _unitOfWork.JobApplication.Add(jobApplicationVm.JobApplication);
            TempData["success"] = "Job Application created successfully";
        }
        else
        {
            await _unitOfWork.JobApplication.Update(jobApplicationVm.JobApplication);
            TempData["success"] = "Job Application updated successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteResume(int id)
    {
        var jobApplication = await _unitOfWork.JobApplication.Get(ja => ja.Id == id);
        if (jobApplication == null)
        {
            return Json(new { success = false, message = "Job Application not found" });
        }

        if (!string.IsNullOrEmpty(jobApplication.Resume))
        {
            DeleteOldResumeFile(jobApplication.Resume);
            jobApplication.Resume = null;
            await _unitOfWork.JobApplication.Update(jobApplication);
            await _unitOfWork.SaveAsync();
        }

        return Json(new { success = true });
    }

    public async Task<IActionResult> DownloadDResume(string fileName)
    {
        var filePath = _webHostEnvironment.WebRootPath + fileName;

        if (!System.IO.File.Exists(filePath)) return NotFound();

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(fileBytes, "application/octet-stream", fileName);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var jobApplication = await _unitOfWork.JobApplication.Get(ja => ja.Id == id);
        if (jobApplication == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(jobApplication.Resume))
        {
            DeleteOldResumeFile(jobApplication.Resume);
        }

        _unitOfWork.JobApplication.Remove(jobApplication);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Job Application deleted successfully";
        return RedirectToAction(nameof(Index));
    }

    private async Task<List<SelectListItem>> GetRecruitmentsList()
    {
        var recruitments = await _unitOfWork.Recruitment.GetAll();
        return recruitments.Select(r => new SelectListItem
        {
            Text = r.JobTitle,
            Value = r.Id.ToString()
        }).ToList();
    }

    private async Task<string> SaveResumeFile(IFormFile file)
    {
        var wwwRootPath = _webHostEnvironment.WebRootPath;
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var applicantsPath = Path.Combine(wwwRootPath, @"images\applicants");
        await using (var fileStream = new FileStream(Path.Combine(applicantsPath, fileName), FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
        
        return @"\images\applicants\" + fileName;
    
        /*string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return uniqueFileName;#1#
    }

    private void DeleteOldResumeFile(string fileName)
    {
        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
    }
}*/