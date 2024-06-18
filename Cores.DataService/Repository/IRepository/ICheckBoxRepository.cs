using Cores.Models;

namespace Cores.DataService.Repository.IRepository;

public interface ICheckBoxRepository : IRepository<CheckBox>
{
    void Update(CheckBox checkBox);
}