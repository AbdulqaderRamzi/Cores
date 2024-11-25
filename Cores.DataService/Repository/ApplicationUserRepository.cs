using System.Threading.Tasks;
using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
{
    private readonly ApplicationDbContext _db;

    public ApplicationUserRepository(ApplicationDbContext db) : base(db) => _db = db;
    

    public async Task Update(ApplicationUser user, List<string> languages, List<WorkSchedule> workSchedules)
    {
        var userFromDb = await _db.ApplicationUsers
            .Include("Languages")
            .Include("WorkSchedules")
            .FirstOrDefaultAsync(u => u.Id == user.Id);
        if (userFromDb is null)
            return;
        userFromDb.FirstName = user.FirstName;
        userFromDb.LastName = user.LastName;
        userFromDb.PhoneNumber = user.PhoneNumber;
        userFromDb.EmergencyNumber = user.EmergencyNumber;
        userFromDb.DateOfBirth = user.DateOfBirth;
        userFromDb.CivilIdNumber = user.CivilIdNumber;
        userFromDb.PassportNumber = user.PassportNumber;
        userFromDb.DepartmentId = user.DepartmentId;
        userFromDb.PositionID = user.PositionID;
        userFromDb.Salary = user.Salary;
        userFromDb.WorkingDaysInMonth = user.WorkingDaysInMonth;

        if (user.ManagerID is null)
        {
            var manager = await _db.ApplicationUsers.Include(a => a.Subordinates).FirstOrDefaultAsync(m => m.Id == userFromDb.ManagerID);
            if (manager is not null)
            {
                if (manager.Subordinates.Count <= 1)
                {
                    manager.IsManager = false;
                }
            }
            userFromDb.ManagerID = null;
        }
        else if (userFromDb.ManagerID != user.ManagerID)
        {
            var newManager = await _db.ApplicationUsers.FirstOrDefaultAsync(m => m.Id == user.ManagerID);
            if (newManager is not null)
            {
                newManager.IsManager = true;
            }
            
            var oldManager = await _db.ApplicationUsers.Include(a => a.Subordinates).FirstOrDefaultAsync(m => m.Id == userFromDb.ManagerID);
            if (oldManager is not null)
            {
                if (oldManager.Subordinates.Count <= 1)
                {
                    oldManager.IsManager = false;
                }
            }
            userFromDb.ManagerID = user.ManagerID;
        } 
        
        userFromDb.BankName = user.BankName;
        userFromDb.BankAccountNumber = user.BankAccountNumber;
        userFromDb.IPAN = user.IPAN;
        userFromDb.Gender = user.Gender;
        userFromDb.PassportExpiredDate = user.PassportExpiredDate;
        userFromDb.ResidenceExpiredDate = user.ResidenceExpiredDate;
        userFromDb.DrivingLicenseExpiredDate = user.DrivingLicenseExpiredDate;
        userFromDb.HealthCardExpiredDate = user.HealthCardExpiredDate;
        userFromDb.ImageUrl = user.ImageUrl;
        userFromDb.AnnualLeaveEntitlement = user.AnnualLeaveEntitlement;
        userFromDb.ResetAnnualLeave = user.ResetAnnualLeave;
        if (languages.Count != 0)   
        {
            /* Update Languages */
            userFromDb.Languages.Clear();
            var languagesFromDb = await _db.Languages.ToListAsync();
            foreach (var lang in languages)
            { 
                var language = languagesFromDb.Find(l => l.Value == lang);
                if (language is null)
                {
                    language = new Language { Value = lang };
                    await _db.Languages.AddAsync(language);
                }
                userFromDb.Languages.Add(language);
            }
        }

        if (workSchedules.Count != 0)
        {
            userFromDb.WorkSchedules.Clear();
            foreach (var workSchedule in workSchedules)
            {
                var workScheduleFromDb = await _db.WorkSchedules.FirstOrDefaultAsync(ws =>
                    ws.DayOfWeek == workSchedule.DayOfWeek &&
                    ws.StartTime == workSchedule.StartTime &&
                    ws.EndTime == workSchedule.EndTime
                );
                if (workScheduleFromDb is null)
                {
                    await _db.WorkSchedules.AddAsync(workSchedule);
                }   
                userFromDb.WorkSchedules.Add(workSchedule);
            }
        }
    }
}