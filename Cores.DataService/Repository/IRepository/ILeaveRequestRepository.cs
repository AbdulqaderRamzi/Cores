using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface ILeaveRequestRepository : IRepository<LeaveRequest>
{
    Task Update(LeaveRequest leaveRequest);
}