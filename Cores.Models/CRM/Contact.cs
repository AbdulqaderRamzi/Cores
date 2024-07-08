using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cores.Models.CRM;

public class Contact
{
    public int Id { get; set; }
    [Required]
    [DisplayName("First Name")]
    public string FirstName { get; set; }
    [DisplayName("Last Name")]
    public string? LastName { get; set; }
    [EmailAddress]
    [Required]
    public string Email { get; set; }
    [Phone]
    [DisplayName("Phone Number")]
    public string? PhoneNumber { get; set; }
    [Required]
    public string Gender { get; set; }
    public string? State{ get; set; }
    public string? City{ get; set; }
    [Required]
    public string Status { get; set; }
    [DisplayName("Street Address")]
    public string? StreetAddress{ get; set; }
    public string? Document { get; set; }
    /*public bool IsCustomer { get; set; }*/
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    public ICollection<Language> Languages { get; set; } = new Collection<Language>();
    public ICollection<Tag> Tags { get; set; } = new Collection<Tag>();
    public ICollection<Purchase> Purchases { get; set; } = new Collection<Purchase>();
    public ICollection<Event> Events { get; set; } = new Collection<Event>();
    public ICollection<Problem> Problems { get; set; } = new Collection<Problem>();
}