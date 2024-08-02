using Cores.Models.Accounting;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class AccountVm
{
    public Account Account { get; set; }
    [ValidateNever]
    public List<SelectListItem> AccountTypes { get; set; }
}