using System.ComponentModel.DataAnnotations;

namespace Cores.Models.Accounting;

public class Tax
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    [Range(0, 100)]
    public decimal Rate { get; set; }
    [Required]
    public string Status { get; set; }
}