using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.Repository;

public class TodoRepository : Repository<Todo>, ITodoRepository
{
    private readonly ApplicationDbContext _db;

    public TodoRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Todo todo)
    {
        var todoFromDb = await _db.Todos.FirstOrDefaultAsync(t => t.Id == todo.Id);
        if (todoFromDb is null)
            return;
        todoFromDb.Title = todo.Title;
        todoFromDb.Description = todo.Description;
        todoFromDb.Priority = todo.Priority;
        todoFromDb.Status = todo.Status;
        todoFromDb.DueDate = todo.DueDate;
        todoFromDb.LastUpdatedDate = DateTime.Now;
        todoFromDb.NotificationDateTime = todo.NotificationDateTime;
        todoFromDb.Comment = todo.Comment;
    }
}