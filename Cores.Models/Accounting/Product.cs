using System.ComponentModel.DataAnnotations;

namespace Cores.Models.Accounting;

public class Product
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    [Display(Name = "Unit Price")]
    public decimal UnitPrice { get; set; }
    
}