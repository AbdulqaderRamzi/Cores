using Cores.Models.HR;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class SalaryVm
{
    public Salary Salary { get; set; }
    [ValidateNever]
    public List<SelectListItem> Employees { get; set; }
}