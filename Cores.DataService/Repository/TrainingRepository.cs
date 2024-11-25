using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class TrainingRepository : Repository<Training>, ITrainingRepository
{
    private readonly ApplicationDbContext _db;

    public TrainingRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Training training)
    {
        var trainingFromDb = await _db.Trainings.FirstOrDefaultAsync(t => t.Id == training.Id);
        if (trainingFromDb is null)
            return;
        trainingFromDb.Name = training.Name;
        trainingFromDb.Description = training.Description;
        trainingFromDb.TrainingDate = training.TrainingDate;
        trainingFromDb.TrainerId = training.TrainerId;
        trainingFromDb.TrainerId = training.TrainerId;
    }
}