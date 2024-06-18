using Cores.Models.CRM;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class CustomerVM
{
    public Customer Customer { get; set; }
    public List<SelectListItem> Tags { get; set; }
}