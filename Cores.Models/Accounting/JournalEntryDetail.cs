using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.Accounting;

public class JournalEntryDetail
{
    public int Id { get; set; }
    
    public int JournalEntryId { get; set; }
    [ForeignKey("JournalEntryId")]
    [ValidateNever]
    public JournalEntry JournalEntry { get; set; }
    
    
    public int AccountId { get; set; }
    [ForeignKey("AccountId")]
    [ValidateNever]
    public Account Account { get; set; }

    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string Description { get; set; }
}