using Cores.Models.Accounting;

namespace Cores.DataService.Repository.IRepository;

public interface ICurrencyRepository : IRepository<Currency>
{
    void Update(Currency currency);
}