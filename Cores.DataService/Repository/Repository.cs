using System.Linq.Expressions;
using Cores.DataService.Data;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;
using IRepository;

public class Repository<T> : IRepository<T> where T : class
{
    private DbSet<T> _dbSet;
    private static readonly char[] Splitor = [','];
    
    private readonly ApplicationDbContext _db;

    public Repository(ApplicationDbContext db)
    {
        _db = db;
        _dbSet = _db.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
    {
        var query = filter is not null ? _dbSet.Where(filter) : _dbSet;
      
        if (string.IsNullOrEmpty(includeProperties)) 
            return await query.ToListAsync();
        
        var properties = includeProperties.Split(Splitor, StringSplitOptions.RemoveEmptyEntries);
        foreach (var property in properties)
            query = query.Include(property);
        
        return await query.ToListAsync();
    }

    public async Task<T?> Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool isTracked = true)
    {
        
        var query = isTracked ? _dbSet : _dbSet.AsNoTracking();
        query = query.Where(filter);
        if (string.IsNullOrEmpty(includeProperties))
            return await query.FirstOrDefaultAsync();

        var properties = includeProperties.Split(Splitor, StringSplitOptions.RemoveEmptyEntries);
        foreach (var property in properties)
            query = query.Include(property);

        return await query.FirstOrDefaultAsync();

    }

    public async Task Add(T entity) => await _db.AddAsync(entity);

    public void Remove(T entity) => _db.Remove(entity);

    public void RemoveRange(IEnumerable<T> entities) => _db.RemoveRange(entities);

}