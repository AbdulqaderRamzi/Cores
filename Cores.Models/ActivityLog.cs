using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models;

public class ActivityLog
{
    public int Id{ get; set; }
    public string ApplicationUserId{ get; set; }
    [ForeignKey("ApplicationUserId")]
    [ValidateNever]
    public ApplicationUser ApplicationUser{ get; set; }
    [NotMapped]
    public string ApplicationUserName{ get; set; } = string.Empty;
    public required string EntityType { get; set; }
    public required string Action { get; set; }
    public required DateTime TimeStamp { get; set; }
    public required string? Property { get; set; }
    public required string? OldValue { get; set; }
    public required string? NewValue { get; set; }
}