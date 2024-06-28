using Cores.Models.Accounting;
using Cores.Models.CRM;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class PurchaseVM
{
    public Purchase Purchase { get; set; }
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public List<Order> Orders { get; set; }
    public List<Product> Products { get; set; }
    public List<SelectListItem>? Customers { get; set; }
    public string SerializedProducts { get; set; }
    public int SelectedCustomerId { get; set; }
}