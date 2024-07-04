using System.Collections.ObjectModel;
using Cores.Models.CRM;

namespace Cores.Models; 

public class Language
{
    public int Id { get; set; }
    public string Value { get; set; }
    public ICollection<ApplicationUser> Employees { get; set; } = new Collection<ApplicationUser>();
    public ICollection<Contact> Contacts { get; set; } = new Collection<Contact>();
} 