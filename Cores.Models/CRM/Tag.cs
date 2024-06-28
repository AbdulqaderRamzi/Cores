using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Cores.Models.CRM;

public class Tag
{
    public int Id{ get; set; }
    [Required]
    public string Name{ get; set; }
    public DateTime DateTime{ get; set; } = DateTime.Now;
    public ICollection<Customer> Customers { get; set; } = new Collection<Customer>();
}