using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models;

public class Notification
{
    public int Id { get; set; }

    [Required]
    public int TodoId { get; set; }
    [ForeignKey("TodoId")]
    [ValidateNever]
    public Todo Todo { get; set; }

    [Required]
    public DateTime NotificationDateTime { get; set; }

    [Required]
    public bool IsRead { get; set; }

    // You might want to add this to track when the notification was created
    public DateTime CreatedAt { get; set; } = DateTime.Now;   
}