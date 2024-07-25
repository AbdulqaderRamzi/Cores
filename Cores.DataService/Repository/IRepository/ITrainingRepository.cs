using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface ITrainingRepository : IRepository<Training>
{
    Task Update(Training training);
}