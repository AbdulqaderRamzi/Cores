using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.CRM;

public class Event
{
    public int Id { get; set; }
    
    public int ContactId { get; set; }
    [ForeignKey("ContactId")]
    [ValidateNever]
    public Contact Contact { get; set; }

    public int EventTypeId { get; set; }
    [ForeignKey("EventTypeId")]
    [ValidateNever]
    public EventType EventType { get; set; }

    public string? ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; }
    
    public string? ModifiedById  { get; set; }
    [ForeignKey("ModifiedById")]
    [ValidateNever]
    public ApplicationUser ModifiedBy { get; set; }

    public DateTime DateTime { get; set; }
    public string Status { get; set; }
    public string? Description { get; set; }
}