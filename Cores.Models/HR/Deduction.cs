using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Cores.Models.HR;

public class Deduction
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }
    [Required]
    public decimal? Amount { get; set; }
    public string? EligibilityCriteria { get; set; }

    public ICollection<EmployeeDeduction> EmployeeDeductions { get; set; } = new Collection<EmployeeDeduction>();  
}