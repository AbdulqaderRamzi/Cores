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
public class RecruitmentController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public RecruitmentController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var recruitments = await _unitOfWork.Recruitment.GetAll
            (includeProperties:"Department,JobApplications");
        return View(recruitments);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        var departments = await _unitOfWork.Department.GetAll();
        var departmentsList = departments.Select(d => new SelectListItem
        {
            Text = d.Name,
            Value = d.Id.ToString()
        }).ToList();
        
        var recruitmentVm = new RecruitmentVm
        {
            Recruitment = new(),
            Departments = departmentsList
        };
    
        if (id is 0)
        {
            return View(recruitmentVm);
        }

        var recruitment = await _unitOfWork.Recruitment.Get(r => r.Id == id, includeProperties:"JobApplications");
        if (recruitment is null)
            return NotFound();
        recruitmentVm.Recruitment = recruitment;
        return View(recruitmentVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(RecruitmentVm recruitmentVm)
    {
        if (!ModelState.IsValid)
        {
            var departments = await _unitOfWork.Department.GetAll();
            var departmentsList = departments.Select(d => new SelectListItem
            {
                Text = d.Name,
                Value = d.Id.ToString()
            }).ToList();
            recruitmentVm.Departments = departmentsList;
            
            return View(recruitmentVm);
            
        }
        if (recruitmentVm.Recruitment.Id is 0)
        {
            await _unitOfWork.Recruitment.Add(recruitmentVm.Recruitment);
            TempData["success"] = "Recruitment added successfully";
        }
        else
        {
            await _unitOfWork.Recruitment.Update(recruitmentVm.Recruitment);
            TempData["success"] = "Recruitment updated successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Delete(int id)
    {
        if (id is 0)
            return BadRequest();
        var recruitment = await _unitOfWork.Recruitment.Get(r => r.Id == id);
        if (recruitment is null)
            return NotFound();
        _unitOfWork.Recruitment.Remove(recruitment);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
}