using Cores.Models.Accounting;

namespace Cores.DataService.Repository.IRepository;

public interface IPayrollRepository : IRepository<Payroll>
{
    Task Update(Payroll payroll);
}