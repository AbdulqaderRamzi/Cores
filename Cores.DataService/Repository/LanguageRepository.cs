using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models;

namespace Cores.DataService.Repository;

public class LanguageRepository : Repository<Language>, ILanguageRepository
{
    private readonly ApplicationDbContext _db;
    public LanguageRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Language language)
    {
        _db.Languages.Update(language);
    }
}