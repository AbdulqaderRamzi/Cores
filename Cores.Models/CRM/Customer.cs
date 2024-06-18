using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cores.Models.CRM;

public class Customer
{
    public int Id { get; set; }
    [Required]
    [DisplayName("First Name")]
    public string FirstName { get; set; }
    [DisplayName("Last Name")]
    public string LastName { get; set; } = string.Empty;
    [EmailAddress]
    [Required]
    public string Email { get; set; }
    [Required]
    [Phone]
    [DisplayName("Phone Number")]
    public string PhoneNumber { get; set; }
    [Required]
    public string Gender { get; set; }
    public string? State{ get; set; }
    public string? City{ get; set; }
    [DisplayName("Street Address")]
    public string? StreetAddress{ get; set; }
    public string? Document { get; set; }
    [Required]
    public bool IsLead { get; set; } = false;
    public DateTime CreatedTime { get; set; } = DateTime.Now;
}