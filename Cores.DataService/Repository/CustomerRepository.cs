using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models;
using Cores.Models.CRM;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    private readonly ApplicationDbContext _db;
    public CustomerRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Customer customer, List<string> languages)
    {
        var customerFromDb = await _db.Customers.Include("Languages").FirstOrDefaultAsync(c => c.Id == customer.Id);
        if (customerFromDb is null) 
            return;
        customerFromDb.FirstName = customer.FirstName;
        customerFromDb.LastName = customer.LastName;
        customerFromDb.City = customer.City;
        customerFromDb.Email = customer.Email;
        customerFromDb.Gender = customer.Gender;
        customerFromDb.State = customer.State;
        customerFromDb.PhoneNumber = customer.PhoneNumber;
        customerFromDb.StreetAddress = customer.StreetAddress;
        customerFromDb.Document = customer.Document;
        
        /* Update Languages */
        customerFromDb.Languages.Clear();
        var languagesFromDb = await _db.Languages.ToListAsync();
        foreach (var lang in languages)
        {
            var language = languagesFromDb.Find(l => l.Value == lang);
            if (language is null)
            {
                language = new Language { Value = lang};
                await _db.Languages.AddAsync(language);
            }
            customerFromDb.Languages.Add(language);
        }
    }
}