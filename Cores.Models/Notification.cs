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
    public DateTime? DateTime { get; set; }
    [Required]
    public bool IsRead { get; set; }
}