using Cores.Models.HR;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class BenefitsRequestVm
{
    public BenefitsRequest BenefitsRequest { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem> BenefitTypes { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem> Relationships { get; set; }
}