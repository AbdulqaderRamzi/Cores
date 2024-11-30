using Cores.Models.HR;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class CompensationRequestVm
{
    public CompensationRequest CompensationRequest { get; set; }
    [ValidateNever]
    public List<SelectListItem> CompensationTypes { get; set; }
}