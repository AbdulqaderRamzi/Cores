using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Cores.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string FirstName{ get; set; }
    public string LastName{ get; set; }
    [Required] 
    public string Career { get; set; }
    [Required] 
    public string Gender { get; set; }
    [Required]
    public int Salary{ get; set; }
    [Required] 
    [DataType(DataType.Date)]  
    [Display(Name = "Date of Birth")]
    public DateOnly DateOfBirth { get; set; }
    [Display(Name = "Street Address")]
    public string? StreetAddress{ get; set; }
    public string? City{ get; set; }
    public string State{ get; set; } = string.Empty;
    public string ImageUrl{ get; set; } = string.Empty;
}