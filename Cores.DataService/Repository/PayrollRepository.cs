using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class PayrollRepository : Repository<Payroll>, IPayrollRepository
{
    private readonly ApplicationDbContext _db;
    public PayrollRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Payroll payroll)
    {
        var payrollFromDb = await _db.Payrolls.FirstOrDefaultAsync(p => p.Id == payroll.Id);
        if (payrollFromDb is null)
            return;
        payrollFromDb.PayPeriodStart = payroll.PayPeriodStart;
        payrollFromDb.PayPeriodEnd = payroll.PayPeriodEnd;
        payrollFromDb.GrossPay = payroll.GrossPay;
        payrollFromDb.TotalDeduction = payroll.TotalDeduction;
        payrollFromDb.TotalBenefit = payroll.TotalBenefit;
        payrollFromDb.NetPay = payroll.NetPay;
        payrollFromDb.PaymentDate = payroll.PaymentDate;
        payrollFromDb.EmployeeId = payroll.EmployeeId;
    }
}