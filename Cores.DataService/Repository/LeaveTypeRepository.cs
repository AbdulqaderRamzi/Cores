using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class LeaveTypeRepository : Repository<LeaveType>, ILeaveTypeRepository
{
    private readonly ApplicationDbContext _db;

    public LeaveTypeRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(LeaveType leaveType)
    {
        var leaveTypeFromDb = await _db.LeaveTypes.FirstOrDefaultAsync(lt => lt.Id == leaveType.Id);
        if (leaveTypeFromDb is null)
            return;
        leaveTypeFromDb.Name = leaveType.Name;
        leaveTypeFromDb.Description = leaveType.Description;
    }
}