using Cores.Models;

namespace Cores.DataService.Repository.IRepository;

public interface ITodoRepository : IRepository<Todo>
{
    Task Update(Todo todo);
}