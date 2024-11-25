using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models;
using Cores.Models.CRM;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class ContactRepository : Repository<Contact>, IContactRepository
{
    private readonly ApplicationDbContext _db;
    public ContactRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Contact contact, List<string> languages)
    {
        var customerFromDb = await _db.Contacts.Include("Languages").Include("Tags")
            .FirstOrDefaultAsync(c => c.Id == contact.Id);
        if (customerFromDb is null)
            return;
        customerFromDb.FirstName = contact.FirstName;
        customerFromDb.LastName = contact.LastName;
        customerFromDb.City = contact.City;
        customerFromDb.Email = contact.Email;
        customerFromDb.Gender = contact.Gender;
        customerFromDb.State = contact.State;
        customerFromDb.PhoneNumber = contact.PhoneNumber;
        customerFromDb.StreetAddress = contact.StreetAddress;
        customerFromDb.Status = contact.Status;
        customerFromDb.Document = contact.Document;
        customerFromDb.Tags.Clear();
        foreach (var tag in contact.Tags)
        {
            customerFromDb.Tags.Add(tag);
        }

        if (languages.Count != 0)
        {
            /* Update Languages */
            customerFromDb.Languages.Clear();
            var languagesFromDb = await _db.Languages.ToListAsync();
            foreach (var lang in languages)
            {
                var language = languagesFromDb.Find(l => l.Value == lang);
                if (language is null)
                {
                    language = new Language { Value = lang };
                    await _db.Languages.AddAsync(language);
                }

                customerFromDb.Languages.Add(language);
            }
        }
    }

    public async Task<Contact?> GetEventWithRelatedData(int contactId)
    {
        return await _db.Contacts
            .Include(c => c.Events).ThenInclude(e => e.EventType)
            .Include(c => c.Events).ThenInclude(e => e.ApplicationUser)
            .Include(c => c.Languages)
            .Include(c => c.Tags)
            //.Include(c => c.Purchases)
            .FirstOrDefaultAsync(c => c.Id == contactId);
    }   
    
    public async Task<(int CustomerCount, int LeadCount)> GetStatusCounts()
    {
        var customerCount = await _db.Contacts.CountAsync(c => c.Status == "customer");
        var leadCount = await _db.Contacts.CountAsync(c => c.Status == "lead");
        return (CustomerCount: customerCount, LeadCount: leadCount);
    }

    public async Task<IDictionary<string, int>> GetMonthlyContacts()
    {
        var oneYearAgo = DateTime.Now.AddYears(-1);
        var contacts = await _db.Contacts
            .Where(c => c.CreatedTime >= oneYearAgo)
            .GroupBy(c => new { c.CreatedTime.Year, c.CreatedTime.Month })
            .Select(g => new
            {
                YearMonth = new DateTime(g.Key.Year, g.Key.Month, 1),
                Count = g.Count()
            })
            .ToDictionaryAsync( 
                k => k.YearMonth.ToString("MMM"),
                v => v.Count
            );

        var allMonths = new[]
        {
            "Jan", "Feb", "Mar", "Apr", "May", "Jun",
            "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        };
        
        return allMonths.ToDictionary(
            month => month,
            month => contacts.TryGetValue(month, out var value) ? value : 0
        );
    }
}