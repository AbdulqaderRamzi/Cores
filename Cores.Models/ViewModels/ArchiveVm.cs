using Cores.Models.HR;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class ArchiveVm
{
    public Archive Archive { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem> Employees { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem> ArchiveTypes { get; set; }

    public bool IsRemoved { get; set; } = false;
}