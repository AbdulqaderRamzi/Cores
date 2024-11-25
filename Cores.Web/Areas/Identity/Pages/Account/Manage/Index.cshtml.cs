#nullable disable

using System.ComponentModel.DataAnnotations;
using Cores.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cores.Web.Areas.Identity.Pages.Account.Manage;

public class IndexModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public IndexModel(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public string Username { get; set; }

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    public class InputModel
    {
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }
    }

    private async Task LoadAsync(ApplicationUser user)
    {
        var userName = await _userManager.GetUserNameAsync(user);
        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        var firstName = user.FirstName;
        var lastName = user.LastName;
        Username = userName;

        Input = new InputModel
        {
            PhoneNumber = phoneNumber,
            FirstName = firstName,
            LastName = lastName
        };
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

        await LoadAsync((ApplicationUser)user);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        var appUser = (ApplicationUser)user;

        if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

        if (!ModelState.IsValid)
        {
            await LoadAsync(appUser);
            return Page();
        }

        var phoneNumber = await _userManager.GetPhoneNumberAsync(appUser);
        if (Input.PhoneNumber != phoneNumber)
        {
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(appUser, Input.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to set phone number.";
                return RedirectToPage();
            }
        }

        // Update fields does not exit in the default identity    
        if (!Input.FirstName.Equals(appUser.FirstName)) appUser.FirstName = Input.FirstName;
        if (!Input.LastName.Equals(appUser.LastName)) appUser.LastName = Input.LastName;

        // Update is needed for the custom fields 
        var updateResult = await _userManager.UpdateAsync(appUser);
        if (!updateResult.Succeeded)
        {
            StatusMessage = "Unexpected error when trying to update user profile.";
            return RedirectToPage();
        }

        await _signInManager.RefreshSignInAsync(appUser);
        StatusMessage = "Your profile has been updated";
        return RedirectToPage();
    }
}