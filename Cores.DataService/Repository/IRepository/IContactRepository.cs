using Cores.Models.CRM;

namespace Cores.DataService.Repository.IRepository;

public interface IContactRepository : IRepository<Contact>
{
    Task Update(Contact contact, List<string> languages);
    Task<Contact?> GetEventWithRelatedData(int contactId);
    Task<(int CustomerCount, int LeadCount)> GetStatusCounts();
    Task<IDictionary<string, int>> GetMonthlyContacts();
}