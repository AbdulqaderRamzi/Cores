using Cores.Models.HR;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class AdministrativeRequestVm
{
    public AdministrativeRequest AdministrativeRequest { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem> RequestTypes { get; set; }
}