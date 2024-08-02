using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cores.Models.Accounting;

public class Vendor
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    [DisplayName("Contact Person Name")]
    public string ContactName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Phone { get; set; }
    public string? State{ get; set; }
    public string? City{ get; set; }
    [DisplayName("Street Address")]
    public string? StreetAddress{ get; set; }
}