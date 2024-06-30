using Cores.Models.Accounting;
using Cores.Models.CRM;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class PurchaseVM
{
    // Store Purchase info 
    public Purchase Purchase { get; set; }
    
    // For Create with purchase
    public int? CustomerId { get; set; } 
    public Customer Customer { get; set; }
    
    // Display Existing Orders For A Particular Purchase 
    public List<Order> Orders { get; set; }
    
    // Products Dropdown list
    public List<Product> Products { get; set; }
    
    // Customers Dropdown list 
    public List<SelectListItem>? Customers { get; set; }
    
    // taking the orders list to HttpPost
    public string SerializedProducts { get; set; }
    
    // Customer id from dropdown 
    public int? SelectedCustomerId { get; set; }
}