using Cores.Models.Accounting;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class PayrollVm
{
    public Payroll Payroll { get; set; }
    [ValidateNever]
    public List<SelectListItem> Employees { get; set; }
}