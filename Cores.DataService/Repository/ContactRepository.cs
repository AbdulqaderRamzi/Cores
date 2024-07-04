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
}