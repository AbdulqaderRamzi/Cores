using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    private readonly ApplicationDbContext _db;

    public DepartmentRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Department department)
    {
        var departmentFromDb = await _db.Departments.FirstOrDefaultAsync(d => d.Id == department.Id);
        if (departmentFromDb is null)
            return;
        departmentFromDb.Name = department.Name;
        departmentFromDb.Location = department.Location;
    }
}