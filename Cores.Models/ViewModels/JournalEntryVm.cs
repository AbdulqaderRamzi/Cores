using Cores.Models.Accounting;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class JournalEntryVm
{
    public JournalEntry JournalEntry { get; set; }
    [ValidateNever]
    public List<SelectListItem> Accounts { get; set; }
    [ValidateNever]
    public List<SelectListItem> Journals { get; set; }
    [ValidateNever]
    public string? EmployeeName { get; set; }
}