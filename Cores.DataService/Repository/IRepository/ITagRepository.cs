using Cores.Models;
using Cores.Models.CRM;

namespace Cores.DataService.Repository.IRepository;

public interface ITagRepository : IRepository<Tag>
{
    Task Update(Tag tag);
}