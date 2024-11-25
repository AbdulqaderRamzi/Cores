using System.Security.Claims;
using Cores.DataService.Repository.IRepository;
using Cores.Models;
using Cores.Models.ViewModels;
using Cores.Utilities;
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
            var role = ExtractRole();
            todoList = await _unitOfWork.Todo.GetAll(t => t.Role == role, includeProperties: "ApplicationUser");
        }
        else
        {
            todoList = await _unitOfWork.Todo.GetAll(t => t.ApplicationUserId == userId, includeProperties: "ApplicationUser");
        }

        ViewBag.ShowAllTasks = showAllTasks;
        return View(todoList);
    }
    
    public async Task<IActionResult> Upsert(int id, bool isDetailsMode = false)
    {
        var todoVm = new TodoVm
        {
            Todo = new(), 
            IsDetailsMode = isDetailsMode 
        };
        
        if (id is 0)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var employeeId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var employee = await _unitOfWork.ApplicationUser.Get(a => a.Id == employeeId);
            todoVm.Todo.ApplicationUserId = employeeId;
            todoVm.Todo.ApplicationUser = employee;
            todoVm.Todo.Role = ExtractRole();
            return View(todoVm);
        }

        var todo = await _unitOfWork.Todo.Get(t => t.Id == id, includeProperties:"ApplicationUser");
        if (todo is null)
        {
            return NotFound();
        }
        todoVm.Todo = todo;
        return View(todoVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(TodoVm todoVm)
    {
        if (!ModelState.IsValid)
        {
            return View(todoVm);
        }
        if (todoVm.Todo.Id is 0)
        {
            todoVm.Todo.CreatedDate = DateTime.Now;
            await _unitOfWork.Todo.Add(todoVm.Todo);
            await _unitOfWork.SaveAsync();
            TempData["success"] = "Todo created successfully";
            
            if (todoVm.Todo.NotificationDateTime is not null)
            {
                var notification = new Notification
                {
                    TodoId = todoVm.Todo.Id,
                    DateTime = todoVm.Todo.NotificationDateTime,
                    IsRead = false
                };
                await _unitOfWork.Notification.Add(notification);
                await _unitOfWork.SaveAsync();
            }
        }
        else
        {
            todoVm.Todo.LastUpdatedDate = DateTime.Now;
            await _unitOfWork.Todo.Update(todoVm.Todo);
            await _unitOfWork.SaveAsync();
            TempData["success"] = "Todo updated successfully";
            var notification = await _unitOfWork.Notification.Get(n => n.TodoId == todoVm.Todo.Id);
            if (notification is not null)
            {
                if (!notification.DateTime.Equals(todoVm.Todo.NotificationDateTime))
                {
                    notification.IsRead = false;
                    notification.DateTime = todoVm.Todo.NotificationDateTime;
                }
            }
            else
            {
                var newNotification = new Notification
                {
                    TodoId = todoVm.Todo.Id,
                    DateTime = todoVm.Todo.NotificationDateTime,
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
    
    private string ExtractRole()
    {
        return User.IsInRole(SD.CRM_ROLE) ? SD.CRM_ROLE :
            User.IsInRole(SD.ACCOUNTING_ROLE) ? SD.ACCOUNTING_ROLE :
            User.IsInRole(SD.HR_ROLE) ? SD.HR_ROLE :
            SD.ADMIN_ROLE;
    }

}