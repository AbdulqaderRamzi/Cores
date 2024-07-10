using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;

public interface IPositionRepository : IRepository<Position>
{
    Task Update(Position position);
}