using Cores.Models.HR;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class PositionVm
{
    public Position Position { get; set; }
    [ValidateNever]
    public List<SelectListItem> Departments { get; set; }
    [ValidateNever]
    public List<SelectListItem> Currencies { get; set; }
}