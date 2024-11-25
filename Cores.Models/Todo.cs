using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models;

public class Todo
{
    public int Id { get; set; }

    public string? ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    [ValidateNever]
    public ApplicationUser? ApplicationUser { get; set; }
    [Required]
    public string Title { get; set; }
    [DisplayName("Task Description")]
    public string? Description { get; set; }
    [Required] 
    public string Priority { get; set; }
    [Required]
    public string Status { get; set; }
    [Required]
    public string Role { get; set; }
    [DisplayName("Created Date")]
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    [DisplayName("Due Date")]
    public DateTime DueDate { get; set; } = DateTime.Now;
    [DisplayName("Last Updated Date")]
    public DateTime? LastUpdatedDate { get; set; }
    [DisplayName("Notify Me At")]
    public DateTime? NotificationDateTime { get; set; }
    public string? Comment { get; set; }
}