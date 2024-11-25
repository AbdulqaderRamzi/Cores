using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class LeaveRequestRepository : Repository<LeaveRequest>, ILeaveRequestRepository
{
    private readonly ApplicationDbContext _db;

    public LeaveRequestRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(LeaveRequest leaveRequest)
    {
        var leaveRequestFromDb = await _db.LeaveRequests.FirstOrDefaultAsync(lr => lr.Id == leaveRequest.Id);
        if (leaveRequestFromDb is null)
            return;
        leaveRequestFromDb.EmployeeId = leaveRequest.EmployeeId;
        leaveRequestFromDb.StartDate = leaveRequest.StartDate;
        leaveRequestFromDb.EndDate = leaveRequest.EndDate;
        leaveRequestFromDb.NumberOfDays = leaveRequest.NumberOfDays;
        leaveRequestFromDb.Reason = leaveRequest.Reason;
        leaveRequestFromDb.LeaveTypeId = leaveRequest.LeaveTypeId;
        leaveRequestFromDb.LeaveStatus = leaveRequest.LeaveStatus;
        leaveRequestFromDb.Document = leaveRequest.Document;
    }
}