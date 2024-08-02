using System.ComponentModel.DataAnnotations;

namespace Cores.Models.Accounting;

public class TaxRate
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public decimal? Rate { get; set; }
    [Required]
    public bool IsActive { get; set; }
}