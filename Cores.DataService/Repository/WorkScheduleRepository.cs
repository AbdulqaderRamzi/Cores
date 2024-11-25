using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class WorkScheduleRepository : Repository<WorkSchedule>, IWorkScheduleRepository
{
    private readonly ApplicationDbContext _db;

    public WorkScheduleRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(WorkSchedule workSchedule)
    {
        var workScheduleFromDb = await _db.WorkSchedules.FirstOrDefaultAsync(ws => ws.Id == workSchedule.Id);
        if (workScheduleFromDb is null)
            return;
        workScheduleFromDb.DayOfWeek = workSchedule.DayOfWeek;
        workScheduleFromDb.StartTime = workSchedule.StartTime;
        workScheduleFromDb.EndTime = workSchedule.EndTime;
    }
}