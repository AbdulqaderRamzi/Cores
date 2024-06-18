using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
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

    public async Task Update(Customer customer)
    {
        var customerFromDb = await _db.Customers.FirstOrDefaultAsync(c => c.Id == customer.Id);
        if (customerFromDb is null) 
            return;
        customerFromDb.FirstName = customer.FirstName;
        customerFromDb.LastName = customer.LastName;
        customerFromDb.City = customer.City;
        customerFromDb.Gender = customer.Gender;
        customerFromDb.State = customer.State;
        customerFromDb.PhoneNumber = customer.PhoneNumber;
        customerFromDb.StreetAddress = customer.StreetAddress;
        customerFromDb.IsLead = customer.IsLead;
        customerFromDb.Document = customer.Document;

    }
}