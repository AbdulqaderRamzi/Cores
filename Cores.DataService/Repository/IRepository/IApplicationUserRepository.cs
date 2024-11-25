using System.Threading.Tasks;
using Cores.Models;
using Cores.Models.HR;

namespace Cores.DataService.Repository.IRepository;



public interface IApplicationUserRepository : IRepository<ApplicationUser>
{
    Task Update(ApplicationUser applicationUser, List<string> languages, List<WorkSchedule> workSchedules);
    /*Task<ApplicationUser> GetRelatedData(string id);*/
}