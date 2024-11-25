using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.CRM;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class TagRepository : Repository<Tag>, ITagRepository
{
    private readonly ApplicationDbContext _db;
    public TagRepository(ApplicationDbContext db) : base(db) => _db = db;
    
    public async Task Update(Tag tag)
    {
        var tagFromDb = await _db.Tags.FirstOrDefaultAsync(t => t.Id == tag.Id);
        if (tagFromDb is null) return;
        tagFromDb.Name = tag.Name;
    }
}