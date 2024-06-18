using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models;

namespace Cores.DataService.Repository;

public class CheckBoxRepository : Repository<CheckBox>, ICheckBoxRepository
{
    private readonly ApplicationDbContext _db;
    
    public CheckBoxRepository(ApplicationDbContext db) : base(db) => _db = db;

    public void Update(CheckBox checkBox)
    {
        _db.Update(checkBox);
    }
}