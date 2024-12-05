using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.HR;

public class Position
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    public int DepartmentId { get; set; }
    [ForeignKey("DepartmentId")]
    [ValidateNever]
    public Department Department { get; set; }

    public int CurrencyId { get; set; }
    [ForeignKey("CurrencyId")]
    [ValidateNever]
    public Department Currency { get; set; }

    public string? JobDescription { get; set; }
    public decimal? AverageSalary { get; set; }

    public ICollection<ApplicationUser> Employees { get; set; } = new Collection<ApplicationUser>();
}