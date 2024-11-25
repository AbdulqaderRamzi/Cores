using Cores.Models.CRM;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class ProblemVm
{
    public Problem Problem { get; set; }
    [ValidateNever]
    public List<SelectListItem> Contacts { get; set; }
    [ValidateNever]
    public List<SelectListItem> ProblemTypes { get; set; }
}