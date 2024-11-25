using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cores.Models.Accounting;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.CRM;

public class Purchase
{
    public int Id { get; set; }
    public DateTime PurchaseDate { get; set; } = DateTime.Now;
    public DateTime InvoiceEndDate { get; set; } = DateTime.Now.AddDays(14);
    [ValidateNever]
    public decimal PurchaseAmount { get; set; }
    
    [Required]
    public int CurrencyId { get; set; }
    [ForeignKey("CurrencyId")]
    [ValidateNever]
    public Currency Currency { get; set; }

    [Required]
    public int PaymentMethodId { get; set; }
    [ForeignKey("PaymentMethodId")]
    [ValidateNever]
    public PaymentMethod PaymentMethod { get; set; }
    
    [Required]
    public string Status { get; set; }

    public int? TaxId { get; set; }
    [ForeignKey("TaxId")]
    [ValidateNever]
    public Tax Tax { get; set; }
    
    public string? Note { get; set; }
    
    [Required]
    public int ContactId { get; set; }
    [ForeignKey("ContactId")]
    [ValidateNever]
    public Contact? Contact { get; set; }

    public ICollection<Order> Orders { get; set; } = new Collection<Order>();
}