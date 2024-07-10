using System.Security.Claims;
using Cores.DataService.Repository.IRepository;
using Cores.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Utility.Controllers;


[Area("Utility")]
[Authorize]
public class TodoController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public TodoController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index(bool showAllTasks = false)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        IEnumerable<Todo> todoList;

        if (showAllTasks)
        {
            todoList = await _unitOfWork.Todo.GetAll(includeProperties: "ApplicationUser");
        }
        else
        {
            todoList = await _unitOfWork.Todo.GetAll(t => t.ApplicationUserId == userId, includeProperties: "ApplicationUser");
        }

        ViewBag.ShowAllTasks = showAllTasks;
        return View(todoList);
    }
    
    public async Task<IActionResult> Upsert(int id, bool details = false)
    {
      
        Todo? todo = new();

        if (id is 0)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var employeeId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var employee = await _unitOfWork.ApplicationUser.Get(a => a.Id == employeeId);
            todo.ApplicationUserId = employeeId;
            todo.ApplicationUser = employee;
            ViewBag.Details = details;
            return View(todo);
        }

        todo = await _unitOfWork.Todo.Get(t => t.Id == id, includeProperties:"ApplicationUser");
        if (todo is null)
        {
            return NotFound();
        }
        ViewBag.Details = details;
        return View(todo);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(Todo todo)
    {
        if (!ModelState.IsValid)
        {
            return View(todo);
        }
        if (todo.Id is 0)
        {
            todo.CreatedDate = DateTime.Now;
            await _unitOfWork.Todo.Add(todo);
            await _unitOfWork.SaveAsync();
            TempData["success"] = "Todo created successfully";
            
            if (todo.NotificationDateTime is not null)
            {
                var notification = new Notification
                {
                    TodoId = todo.Id,
                    DateTime = todo.NotificationDateTime,
                    IsRead = false
                };
                await _unitOfWork.Notification.Add(notification);
                await _unitOfWork.SaveAsync();
            }
        }
        else
        {
            todo.LastUpdatedDate = DateTime.Now;
            await _unitOfWork.Todo.Update(todo);
            await _unitOfWork.SaveAsync();
            TempData["success"] = "Todo updated successfully";
            var notification = await _unitOfWork.Notification.Get(n => n.TodoId == todo.Id);
            if (notification is not null)
            {
                if (!notification.DateTime.Equals(todo.NotificationDateTime))
                {
                    notification.IsRead = false;
                    notification.DateTime = todo.NotificationDateTime;
                }
            }
            else
            {
                var newNotification = new Notification
                {
                    TodoId = todo.Id,
                    DateTime = todo.NotificationDateTime,
                    IsRead = false
                };
                await _unitOfWork.Notification.Add(newNotification);
            }
            await _unitOfWork.SaveAsync();   
        }
       
        
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return BadRequest();
        }
        var todo = await _unitOfWork.Todo.Get(t => t.Id == id);
        if (todo == null)
        {
            return NotFound();
        }
        _unitOfWork.Todo.Remove(todo);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Todo deleted successfully";
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> CloneTask(int? taskId)
    {

        if (taskId is null)
        {
            return BadRequest();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var taskToClone = await _unitOfWork.Todo.Get(t => t.Id == taskId);

        if (taskToClone == null)
        {
            return NotFound();
        }

        var clonedTask = new Todo
        {
            Title = taskToClone.Title,
            Description = taskToClone.Description,
            Priority = taskToClone.Priority,
            Status = taskToClone.Status,
            DueDate = taskToClone.DueDate,
            NotificationDateTime = taskToClone.NotificationDateTime,
            ApplicationUserId = userId,
            CreatedDate = DateTime.Now
        };

        await _unitOfWork.Todo.Add(clonedTask);
        _unitOfWork.Todo.Remove(taskToClone);
        await _unitOfWork.SaveAsync();

        TempData["success"] = "Task successfully cloned";
        return RedirectToAction(nameof(Index));
    }

}