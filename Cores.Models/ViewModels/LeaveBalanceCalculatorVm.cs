using System.ComponentModel.DataAnnotations.Schema;
using Cores.Models.HR;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.ViewModels;

public class LeaveBalanceCalculatorVm
{
    public string EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public double CurrentBalance { get; set; }
    public double AccrualRate { get; set; }
    public double RequestedDays { get; set; }
    public double? ProjectedBalance { get; set; }
}