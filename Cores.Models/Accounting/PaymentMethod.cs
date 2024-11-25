using System.ComponentModel.DataAnnotations;

namespace Cores.Models.Accounting;

public class PaymentMethod
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
}