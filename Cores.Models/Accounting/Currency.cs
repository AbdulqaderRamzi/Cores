using System.ComponentModel.DataAnnotations;

namespace Cores.Models.Accounting;

public class Currency
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Code { get; set; }
}