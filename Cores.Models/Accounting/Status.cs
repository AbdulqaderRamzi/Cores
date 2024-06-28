using System.ComponentModel.DataAnnotations;

namespace Cores.Models.Accounting;

public class Status
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
}