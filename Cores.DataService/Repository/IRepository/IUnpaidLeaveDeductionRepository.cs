using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IUnpaidLeaveDeductionRepository : IRepository<UnpaidLeaveDeduction>
{
    Task Update(UnpaidLeaveDeduction unpaidLeaveDeduction);
}