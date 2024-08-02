using Cores.Models.Accounting;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class JournalVm
{
    public Journal Journal { get; set; }
    [ValidateNever]
    public List<SelectListItem> JournalTypes { get; set; }
}