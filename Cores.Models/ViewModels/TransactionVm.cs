using Cores.Models.Accounting;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class TransactionVm
{
    public Transaction Transaction { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem> Accounts{ get; set; }
    [ValidateNever]
    public string EmployeeName { get; set; }
    /*
    public string SerializedProducts { get; set; }
*/
}