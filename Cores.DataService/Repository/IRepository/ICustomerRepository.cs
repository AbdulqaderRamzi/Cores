using Cores.Models.CRM;

namespace Cores.DataService.Repository.IRepository;

public interface ICustomerRepository : IRepository<Customer>
{
    Task Update(Customer customer, List<string> languages);
}