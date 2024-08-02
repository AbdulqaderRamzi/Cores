using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.Accounting;

public class Transaction
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    [Required]
    public decimal? Amount { get; set; }
    
    public int DebitAccountId { get; set; }
    [ForeignKey("DebitAccountId")]
    [ValidateNever]
    public Account DebitAccount { get; set; }
    
    public int CreditAccountId { get; set; }
    [ForeignKey("CreditAccountId")]
    [ValidateNever]
    public Account CreditAccount { get; set; }

    public string ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; }
}