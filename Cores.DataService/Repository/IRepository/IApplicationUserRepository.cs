using Cores.Models;

namespace Cores.DataService.Repository.IRepository;



public interface IApplicationUserRepository : IRepository<ApplicationUser>
{
    Task Update(ApplicationUser applicationUser);
}