using System.ComponentModel.DataAnnotations;

namespace Cores.Models.Accounting;

public class AccountType
{
    public int Id { get; set; }
    [Required]
    public string Type { get; set; }
    public string? Description { get; set; }
}