using Cores.Models;

namespace Cores.DataService.Repository.IRepository;

public interface ILanguageRepository : IRepository<Language>
{
    Task Update(Language language);
}