using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Cores.Models.HR;

public class Benefit
{
    
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }
    [Required]
    public decimal? Amount { get; set; }
    public string? EligibilityCriteria { get; set; }

    public ICollection<EmployeeBenefit> EmployeeBenefits { get; set; } = new Collection<EmployeeBenefit>();   
}