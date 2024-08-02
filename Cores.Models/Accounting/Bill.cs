using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.Accounting;

public class Bill
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public DateTime DueDate { get; set; } = DateTime.Now.AddDays(14);

    public int VendorId { get; set; }
    [ForeignKey("VendorId")]
    [ValidateNever]
    public Vendor Vendor { get; set; }
    
    [Required]
    public decimal? TotalAmount { get; set; }
    [Required]
    public decimal? PaidAmount { get; set; }
    
    public string Status { get; set; } // e.g., Draft, Received, Paid, Overdue
}