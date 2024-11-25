using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class VendorRepository : Repository<Vendor>, IVendorRepository
{
    private readonly ApplicationDbContext _db;

    public VendorRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Vendor vendor)
    {
        var vendorFromDb = await _db.Vendors.FirstOrDefaultAsync(v => v.Id == vendor.Id);
        if (vendorFromDb is null)
            return;
        vendorFromDb.Name = vendor.Name;
        vendorFromDb.ContactName = vendor.ContactName;
        vendorFromDb.Email = vendor.Email;
        vendorFromDb.Phone = vendor.Phone;
        vendorFromDb.State = vendor.State;
        vendorFromDb.City = vendor.City;
        vendorFromDb.StreetAddress = vendor.StreetAddress;
    }
}