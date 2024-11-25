using Cores.DataService.Repository.IRepository;
using Cores.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.ViewComponents;

public class ExpiryDateViewComponent : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly DateTime _today;

    public ExpiryDateViewComponent(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _today = DateTime.Now;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var employees = await _unitOfWork.ApplicationUser.GetAll(
            e => e.PassportExpiredDate < _today || 
                e.ResidenceExpiredDate < _today ||
                e.HealthCardExpiredDate < _today || 
                e.DrivingLicenseExpiredDate < _today
        );
        // name, expiry dates
        Dictionary<ApplicationUser, List<string>> dic = [];
        var total = 0;
        foreach (var employee in employees)
        {
            List<string> list = [];
            
            AddAndCountIfExpired(employee.PassportExpiredDate, "Passport", list, ref total);
            AddAndCountIfExpired(employee.ResidenceExpiredDate, "Residence", list, ref total);
            AddAndCountIfExpired(employee.HealthCardExpiredDate, "Health Card", list, ref total);
            AddAndCountIfExpired(employee.DrivingLicenseExpiredDate, "Driving License", list, ref total);
            
            dic.Add(employee, list);
        }
        ViewBag.Total = total;
        return View(dic);
    }

    private void AddAndCountIfExpired(DateTime? date, string idName, List<string> list, ref int total)
    {
        if (date > _today) return;
        list.Add(idName);
        total++;
    }   
}