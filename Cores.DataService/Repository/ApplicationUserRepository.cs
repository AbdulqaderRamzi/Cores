using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
{
    private readonly ApplicationDbContext _db;

    public ApplicationUserRepository(ApplicationDbContext db) : base(db) => _db = db;
    

    public async Task Update(ApplicationUser applicationUser)
    {
        _db.Update(applicationUser);
    }

  
}