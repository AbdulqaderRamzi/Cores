using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cores.DataService.Repository.IRepository;
using Cores.Models;
using Cores.Models.HR;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Cores.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.ADMIN_ROLE + "," + SD.HR_ROLE)]
public class EmployeeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public EmployeeController(IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager, 
        UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _roleManager = roleManager;
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        var applicationUsers = await _unitOfWork.ApplicationUser
            .GetAll(e => e.Email != "admin@cores.com");
        return View(applicationUsers);
    }

    public async Task<IActionResult> Update(string? id)
    {
        if (id is null)
            return BadRequest();
        var employee = await _unitOfWork.ApplicationUser.Get(a => a.Id == id,
            includeProperties: "Languages,LeaveRequests.LeaveType,Attendances,Salaries,EmployeeTrainings.Training.Trainer,EmployeeBenefits.Benefit,Documents.ArchiveType,WorkSchedules");
        if (employee is null)
            return NotFound();
            
        var subordinates = await _unitOfWork.ApplicationUser.GetAll(a => a.ManagerID == id, includeProperties:"Position.Department");
        employee.Subordinates = subordinates.ToList();

        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            MaxDepth = 64
        };
        var serializedEmployee = JsonSerializer.Serialize(employee, jsonOptions);
       
        EmployeeVm employeeVm = new()
        {   
            Employee = employee,
            SerializedEmployee = serializedEmployee
        };
        var roles = await _userManager.GetRolesAsync(employee);
        if (roles.Any())
        {
            employeeVm.Role = roles[0];
        }
        
        await FillSelectionData(employeeVm);
      
        return View(employeeVm);
    }

    [HttpPost]
    public async Task<IActionResult> Update(EmployeeVm employeeVm, List<string> languages, IFormFile? file, string schedules)
    {
        if (!ModelState.IsValid)
        {
            await FillSelectionData(employeeVm);
            return View(employeeVm);
        }

        if (!string.IsNullOrEmpty(employeeVm.Role))
        {
            var user = await _userManager.FindByIdAsync(employeeVm.Employee.Id);
            var applicationUser = await _unitOfWork.ApplicationUser.Get(e => e.Id == employeeVm.Employee.Id);
            if (user is null || applicationUser is null)
                return NotFound();
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any()) 
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }
            await _userManager.AddToRoleAsync(user, employeeVm.Role);
            applicationUser.Role = employeeVm.Role;
            await _userManager.UpdateAsync(user);
            await _unitOfWork.SaveAsync();
        }
        
        var workSchedules = JsonConvert.DeserializeObject<List<WorkSchedule>>(schedules);

        var wwwRootPath = _webHostEnvironment.WebRootPath;
        if (file is not null)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var contactPath = Path.Combine(wwwRootPath, @"images\employees");
            await using (var fileStream = new FileStream(Path.Combine(contactPath, fileName), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            if (!string.IsNullOrEmpty(employeeVm.Employee.ImageUrl))
            {
                var oldImagePath = Path.Combine(wwwRootPath, employeeVm.Employee.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                    System.IO.File.Delete(oldImagePath);
            }
            employeeVm.Employee.ImageUrl = @"\images\employees\" + fileName;
        }
        else
        {
            var employee = await _unitOfWork.ApplicationUser.Get(e => e.Id == employeeVm.Employee.Id);
            if (employee is null)
                return NotFound();
            if (employee.ImageUrl is not null)
            {
                employeeVm.Employee.ImageUrl = employee.ImageUrl;
            }
        }
      
        await _unitOfWork.ApplicationUser.Update(employeeVm.Employee, languages, workSchedules);
        await _unitOfWork.SaveAsync();
        
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(string? id)
    {
        if (id is null)
            return RedirectToAction(nameof(Index));
        var employee = await _unitOfWork.ApplicationUser.Get(a => a.Id == id, "Languages");
        if (employee is null)
            return RedirectToAction(nameof(Index));
        return View(employee);
    }

    public async Task<IActionResult> ResetPass(string? userEmail)
    {
        if (userEmail is null)
            return BadRequest();
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user is null)
            return NotFound();
        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        return RedirectToPage("/Account/ResetPassword", new { area = "Identity", code, email = userEmail, isAdminOrHr = true});
    }

    private async Task FillSelectionData(EmployeeVm employeeVm)
    {
        var employees = await _unitOfWork.ApplicationUser.GetAll();
        var departments = await _unitOfWork.Department.GetAll();
        var positions = await _unitOfWork.Position.GetAll();
    
        employeeVm.Roles = _roleManager.Roles.Select(r => r.Name).Select(n => new SelectListItem
        {
            Text = n,
            Value = n
        }).ToList();

        employeeVm.Employees = employees.Select(e => new SelectListItem
        {
            Text = $"{e.FirstName} {e.LastName}",
            Value = e.Id
        }).ToList();
        employeeVm.Departments = departments.Select(d => new SelectListItem
        {
            Text = d.Name,
            Value = d.Id.ToString()
        }).ToList();
        employeeVm.Positions = positions.Select(p => new SelectListItem
        {
            Text = p.Title,
            Value = p.Id.ToString()
        }).ToList();

        employeeVm.LanguagesOptions =
        [
            new CheckBox { Value = "English", isChecked = false },
            new CheckBox { Value = "Arabic", isChecked = false },
            new CheckBox { Value = "Spanish", isChecked = false },
            new CheckBox { Value = "French", isChecked = false }
        ];
        employeeVm.LanguagesOptions.ForEach(lo =>
        {
            foreach (var language in employeeVm.Employee.Languages)
            {
                if (lo.Value.Equals(language.Value))
                {
                    lo.isChecked = true;
                }
            }
        });
    }

    #region API CALL
    
    [HttpPost]
    public async Task<IActionResult> LockUnlock(string? id)
    {
        if (string.IsNullOrEmpty(id))
            return Json(new { success = false, message = "Invalid user ID" });

        var user = await _unitOfWork.ApplicationUser.Get(a => a.Id == id);

        if (user is null)
            return Json(new { success = false, message = "User not found" });

        var isLocked = user.LockoutEnd is not null && user.LockoutEnd > DateTime.Now;
        user.LockoutEnd = isLocked ? DateTime.Now : DateTime.Now.AddYears(1000);

        await _unitOfWork.SaveAsync();

        return Json(new { success = true, message = "Operation Success", isLocked = !isLocked });
    }

    #endregion
}