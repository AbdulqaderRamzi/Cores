using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class EmployeeTrainingRepository : Repository<EmployeeTraining>, IEmployeeTrainingRepository
{
    private readonly ApplicationDbContext _db;
    
    public EmployeeTrainingRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(EmployeeTraining employeeTraining)
    {
        var employeeTrainingFromDb = await _db.EmployeeTrainings.FirstOrDefaultAsync(et => et.Id == employeeTraining.Id);
        if (employeeTrainingFromDb is null) 
            return;
        employeeTrainingFromDb.EmployeeId = employeeTraining.EmployeeId;
        employeeTrainingFromDb.TrainingId = employeeTraining.TrainingId;
        employeeTrainingFromDb.CompletionStatus = employeeTraining.CompletionStatus;
        employeeTrainingFromDb.CompletionDate = employeeTraining.CompletionDate;
    }
}