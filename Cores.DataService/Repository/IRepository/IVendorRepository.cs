using Cores.Models.Accounting;

namespace Cores.DataService.Repository.IRepository;

public interface IVendorRepository : IRepository<Vendor>
{
    Task Update(Vendor vendor);
}