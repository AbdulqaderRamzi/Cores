using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models;


public class MessagePayload
{
    public int Id { get; set; }
    public string? SenderId { get; set; }
    [ForeignKey("SenderId")] 
    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; }
    public string RecipientEmail{ get; set; }
    public string Content { get; set; }
    public DateTime SentAt{ get; set; } = DateTime.Now;
    public bool Seen{ get; set; }
}