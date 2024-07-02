using Cores.Models.CRM;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class CustomerVm
{
    public Customer Customer { get; set; }
    [ValidateNever]
    public List<SelectListItem> Tags { get; set; }
    [ValidateNever]
    public List<int> SelectedTagIds { get; set; }
    [ValidateNever]
    public List<CheckBox> LanguagesOptions { get; set; }
    [ValidateNever]
    public List<Purchase> Purchases{ get; set; }
}