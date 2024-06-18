// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Cores.DataService.Repository.IRepository;
using Cores.Models;
using Cores.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;

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

        [Required]
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

        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public string Career { get; set; }
        [Required] public int Salary { get; set; }
        [Required] public string Gender { get; set; }

        public List<CheckBox> LanguagesOptions { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateOnly DateOfBirth { get; set; }

        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }


    public async Task OnGetAsync(string returnUrl = null)
    {
        if (!_roleManager.RoleExistsAsync(SD.ADMIN_ROLE).GetAwaiter().GetResult())
        {
            _roleManager.CreateAsync(new IdentityRole(SD.ADMIN_ROLE)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.ACCOUNTING_ROLE)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.HR_ROLE)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.CRM_ROLE)).GetAwaiter().GetResult();
        }

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
            ]
        };

        ReturnUrl = returnUrl;
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(IFormFile file, List<string> languages, string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        if (ModelState.IsValid)
        {
            var user = CreateUser();

            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user,
                Input.Email, CancellationToken.None);
            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.Career = Input.Career;
            user.Salary = Input.Salary;
            user.Gender = Input.Gender;
            user.City = Input.City;
            user.State = Input.State;
            user.StreetAddress = Input.StreetAddress;
            user.PhoneNumber = Input.PhoneNumber;
            user.DateOfBirth = Input.DateOfBirth;
            // Fetching the image and save it
            var wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file is not null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var employeePath = Path.Combine(wwwRootPath, @"images\employees");
                var filePath = Path.Combine(employeePath, fileName);

                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                /*if (!string.IsNullOrEmpty(Input.ImageUrl)) {
                    var oldImagePath = Path.Combine(wwwRootPath, Input.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }*/
                Input.ImageUrl = @"\images\employees\" + fileName;
            }

            user.ImageUrl = Input.ImageUrl;
            foreach (var lang in languages)
            {
                var checkbox = new CheckBox
                {
                    Value = lang,
                    isChecked = true,
                    EmployeeId = user.Id
                };
                await _unitOfWork.CheckBox.Add(checkbox);
            }

            var result = await _userManager.CreateAsync(user, Input.Password);
            await _unitOfWork.SaveAsync();

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");
                if (!string.IsNullOrEmpty(Input.Role)) await _userManager.AddToRoleAsync(user, Input.Role);
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    null,
                    new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    Request.Scheme);

                await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });

                if (User.IsInRole(SD.ADMIN_ROLE))
                    TempData["success"] = "The account has created successfully";
                else
                    await _signInManager.SignInAsync(user, false); // log the user in when register into the account
                return LocalRedirect($"{returnUrl}Admin/Employee/Index");
            }

            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        }

        Input = new InputModel
        {
            RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
            {
                Text = i,
                Value = i
            })
        };
        // If we got this far, something failed, redisplay form
        return Page();
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