using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.CRM;

public class Purchase
{
    public int Id { get; set; }
    public DateTime PurchaseDate { get; set; } = DateTime.Now;
    [ValidateNever]
    public decimal PurchaseAmount { get; set; }
    [Required]
    public string Currency { get; set; }
    [Required]
    public string Status { get; set; }
    [Required]
    public string PaymentMethod { get; set; }
    public string? Note { get; set; }
    public int ContactId { get; set; }
    [ForeignKey("ContactId")]
    [ValidateNever]
    public Contact? Contact { get; set; }

    public ICollection<Order> Orders { get; set; } = new Collection<Order>();
}