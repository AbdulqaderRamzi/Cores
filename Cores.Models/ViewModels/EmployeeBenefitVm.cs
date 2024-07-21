using Cores.Models.HR;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class EmployeeBenefitVm
{
    public EmployeeBenefit EmployeeBenefit { get; set; }
    [ValidateNever]
    public List<SelectListItem> Employees { get; set; }
    [ValidateNever]
    public List<SelectListItem> Benefits { get; set; }
}