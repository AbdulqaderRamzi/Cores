using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.Accounting;

public class Account
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int AccountTypeId { get; set; }
    [ForeignKey("AccountTypeId")]
    [ValidateNever]
    public AccountType AccountType { get; set; }
    [Required]
    public decimal? Balance { get; set; }
    [Required]
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}