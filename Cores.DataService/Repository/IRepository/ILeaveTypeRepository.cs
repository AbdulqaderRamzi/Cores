using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface ILeaveTypeRepository : IRepository<LeaveType>
{
    Task Update(LeaveType leaveType);
}