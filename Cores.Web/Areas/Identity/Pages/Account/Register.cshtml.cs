// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Cores.DataService.Repository.IRepository;
using Cores.Models;
using Cores.Models.HR;
using Cores.Utilities;
using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using QuestPDF.Fluent;

namespace Cores.Web.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserStore<IdentityUser> _userStore;
    private readonly IUserEmailStore<IdentityUser> _emailStore;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IEmailSender _emailSender;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IUnitOfWork _unitOfWork;


    public RegisterModel(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IUserStore<IdentityUser> userStore,
        SignInManager<IdentityUser> signInManager,
        ILogger<RegisterModel> logger,
        IEmailSender emailSender,
        IUnitOfWork unitOfWork,
        IWebHostEnvironment webHostEnvironment)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _logger = logger;
        _emailSender = emailSender;
        _webHostEnvironment = webHostEnvironment;
        _unitOfWork = unitOfWork;
    }

    [BindProperty] public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required] public string Role { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }
        public List<SelectListItem> Employees { get; set; }
        public List<SelectListItem> Departments { get; set; }
        public List<SelectListItem> Positions { get; set; }


        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public int Salary { get; set; }
        [Required] public string Gender { get; set; }
        public string Document { get; set; }


        public List<CheckBox> LanguagesOptions { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }



        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string ManagerId { get; set; }
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string IPAN { get; set; }
        public string CivilIdNumber { get; set; }
        public string PassportNumber { get; set; }
        public DateTime? PassportExpiredDate { get; set; }
        public DateTime? ResidenceExpiredDate { get; set; }
        public DateTime? HealthCardExpiredDate { get; set; }
        public DateTime? DrivingLicenseExpiredDate { get; set; }
        public DateTime? StartAt { get; set; } = DateTime.Now;
        public DateTime? DateOfBirth { get; set; }
       
        public string EmergencyNumber { get; set; }
        public double AnnualLeaveBalance { get; set; }
        public int? AnnualLeaveEntitlement{ get; set; }
        public byte? WorkingDaysInMonth { get; set; }
        public bool ResetAnnualLeave { get; set; }
        
    }
    
    public async Task OnGetAsync(string returnUrl = null)
    {
        var employees = await _unitOfWork.ApplicationUser.GetAll();
        var departments = await _unitOfWork.Department.GetAll();
        var positions = await _unitOfWork.Position.GetAll();
        Input = new InputModel
        {
            RoleList = _roleManager.Roles.Where(r => r.Name != SD.ADMIN_ROLE).Select(x => x.Name).Select(i => new SelectListItem
            {
                Text = i,
                Value = i
            }),
            LanguagesOptions =
            [
                new CheckBox { Value = "English", isChecked = false },
                new CheckBox { Value = "Arabic", isChecked = false },
                new CheckBox { Value = "Spanish", isChecked = false },
                new CheckBox { Value = "French", isChecked = false }
            ],
            Employees = employees.Select(e => new SelectListItem
            {
                Text = $"{e.FirstName} {e.LastName}",
                Value = e.Id
            }).ToList(),
            Departments = departments.Select(d => new SelectListItem
            {
                Text = d.Name,
                Value = d.Id.ToString()
            }).ToList(),
            Positions = positions.Select(p => new SelectListItem
            {
                Text = p.Title,
                Value = p.Id.ToString()
            }).ToList(),
            AnnualLeaveEntitlement = 30,
            ResetAnnualLeave = false
        };

        ReturnUrl = returnUrl;
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }
 
    public async Task<IActionResult> OnPostAsync(IFormFile file, IFormFile document, List<string> languages, string schedules, string returnUrl = null)
{
     returnUrl ??= Url.Content("~/");
    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    if (ModelState.IsValid)
    {
        var user = CreateUser();
        
        await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
        user.FirstName = Input.FirstName;
        user.LastName = Input.LastName;
        user.Salary = Input.Salary;
        user.Gender = Input.Gender;
        user.City = Input.City;
        user.State = Input.State;
        user.StreetAddress = Input.StreetAddress;
        user.PhoneNumber = Input.PhoneNumber;
        user.DateOfBirth = Input.DateOfBirth;
        user.ManagerID = Input.ManagerId;
        user.DepartmentId = Input.DepartmentId;
        user.PositionID = Input.PositionId;
        user.BankName = Input.BankName;
        user.BankAccountNumber = Input.BankAccountNumber;
        user.IPAN = Input.IPAN;
        user.CivilIdNumber = Input.CivilIdNumber;
        user.PassportNumber = Input.PassportNumber;
        user.PassportExpiredDate = Input.PassportExpiredDate;
        user.ResidenceExpiredDate = Input.ResidenceExpiredDate;
        user.HealthCardExpiredDate = Input.HealthCardExpiredDate;
        user.DrivingLicenseExpiredDate = Input.DrivingLicenseExpiredDate;
        user.StartAt = Input.StartAt;
        user.EmergencyNumber = Input.EmergencyNumber;
        user.AnnualLeaveEntitlement = Input.AnnualLeaveEntitlement;
        user.WorkingDaysInMonth = Input.WorkingDaysInMonth;
        user.ResetAnnualLeave = Input.ResetAnnualLeave;

        // Set a temporary password
        var tempPassword = $"Cores123${Guid.NewGuid().ToString()}";
        var result = await _userManager.CreateAsync(user, tempPassword);

        if (result.Succeeded)
        {
            _logger.LogInformation("User account created.");

            if (!string.IsNullOrEmpty(Input.Role))
            {
                await _userManager.AddToRoleAsync(user, Input.Role);
                user.Role = Input.Role;
            }

            foreach (var lang in languages)
            {
                var language = await _unitOfWork.Language.Get(l => l.Value == lang);
                if (language == null)
                {
                    language = new Language { Value = lang };
                    await _unitOfWork.Language.Add(language);
                }
                user.Languages.Add(language);
            }

            if (document is not null)
            {
                var doc = new Archive
                {
                    EmployeeId = user.Id,
                    Path = await ProcessAndSaveImage(document, "archives"),
                    UploadDate = DateOnly.FromDateTime(DateTime.Now),
                    Description = "Uploaded While Registering The Employee"
                };
                await _unitOfWork.Archive.Add(doc);
            }

            var workSchedules = JsonConvert.DeserializeObject<List<WorkSchedule>>(schedules);
            foreach (var schedule in workSchedules)
            {
                var workScheduleFromDb = await _unitOfWork.WorkSchedule.Get(ws =>
                    ws.DayOfWeek == schedule.DayOfWeek &&
                    ws.StartTime == schedule.StartTime &&
                    ws.EndTime == schedule.EndTime
                );
                if (workScheduleFromDb is null)
                {
                    await _unitOfWork.WorkSchedule.Add(schedule);
                }
                user.WorkSchedules.Add(schedule);
            }

            if (file is not null)
            {
                var imageUrl = await ProcessAndSaveImage(file,"employees");
                user.ImageUrl = imageUrl;
            }

            if (user.ManagerID is not null)
            {
                var manager = await _unitOfWork.ApplicationUser.Get(m => m.Id == user.ManagerID);
                if (manager is not null)
                {
                    manager.IsManager = true;
                }
            }

            await _userManager.UpdateAsync(user);
            await _unitOfWork.SaveAsync();
            
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var resetPasswordUrl = Url.Page(
                "/Account/ResetPassword",
                null,
                new { area = "Identity", code, email = user.Email },
                Request.Scheme);

            // Enqueue background job
            BackgroundJob.Enqueue(() => SendWelcomeEmailAsync(user.Id, tempPassword, resetPasswordUrl));

            if (User.IsInRole(SD.ADMIN_ROLE))
            {
                TempData["success"] = "The account has been created.";
            }
            else
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return LocalRedirect($"{returnUrl}Admin/Employee/Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    // If we got this far, something failed, redisplay form
    await PopulateFormData();
    return Page();
}
    

public async Task SendWelcomeEmailAsync(string userId, string tempPassword, string resetPasswordUrl)
{
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
        _logger.LogError($"User not found for ID: {userId}");
        return;
    }

    var subject = "Welcome To Our Company";
    var message = $"""
                   
                           <!DOCTYPE html>
                           <html>
                               <head>
                                   <meta charset="UTF-8">
                               </head>
                               <body>
                                   <p style="margin-bottom: 20px;">Dear {((ApplicationUser)user).FirstName} {((ApplicationUser)user).LastName},</p>
                                   
                                   <p style="margin-bottom: 20px;">Welcome to our company! Your account has been successfully created.</p>
                                   
                                   <p style="margin-bottom: 20px;">Your login credentials are:</p>
                                   <p>Username: {user.Email}</p>
                                   <p style="margin-bottom: 20px;">
                                        You can set your password by clicking on <a href='{HtmlEncoder.Default.Encode(resetPasswordUrl)}'>Set Password</a>
                                   </p>
                                   <p style="margin-bottom: 20px;">If you have any questions or need assistance, please don't hesitate to contact our support team.</p>
                                   
                                   <p>Best regards,</p>
                                   <p>System Team</p>
                               </body>
                           </html>
                   """;

    await _emailSender.SendEmailAsync(user.Email!, subject, message);
}

private async Task PopulateFormData()
{
    var employees = await _unitOfWork.ApplicationUser.GetAll();
    var departments = await _unitOfWork.Department.GetAll();
    var positions = await _unitOfWork.Position.GetAll();
    Input = new InputModel
    {
        RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
        {
            Text = i,
            Value = i
        }),
        
        LanguagesOptions =
        [
            new CheckBox { Value = "English", isChecked = false },
            new CheckBox { Value = "Arabic", isChecked = false },
            new CheckBox { Value = "Spanish", isChecked = false },
            new CheckBox { Value = "French", isChecked = false }
        ],
        
        Employees = employees.Select(e => new SelectListItem
        {
            Text = $"{e.FirstName} {e.LastName}",
            Value = e.Id
        }).ToList(),
        
        Departments = departments.Select(d => new SelectListItem
        {
            Text = d.Name,
            Value = d.Id.ToString()
        }).ToList(),
        
        Positions = positions.Select(p => new SelectListItem
        {
            Text = p.Title,
            Value = p.Id.ToString()
        }).ToList(),
    };
}

private async Task<string> ProcessAndSaveImage(IFormFile file, string dir)
{
    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
    var employeePath = Path.Combine(_webHostEnvironment.WebRootPath, @$"images\{dir}");
    var filePath = Path.Combine(employeePath, fileName);

    using (var fileStream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(fileStream);
    }

    return @$"\images\{dir}\{fileName}";
}
    
    private ApplicationUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                                                $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                                                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        }
    }

    private IUserEmailStore<IdentityUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
            throw new NotSupportedException("The default UI requires a user store with email support.");
        return (IUserEmailStore<IdentityUser>)_userStore;
    }
}