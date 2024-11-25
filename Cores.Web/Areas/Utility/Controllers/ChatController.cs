using System.Security.Claims;
using Cores.DataService.Repository.IRepository;
using Cores.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Utility.Controllers;

[Area("Utility")]
[Authorize]
public class ChatController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ChatController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> JoinChat()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity!;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
        return View(new UserConnection
        {
            Username = string.Concat(user?.FirstName, " ", user?.LastName)
        });
    }

    [HttpPost]
    public IActionResult JoinChat(UserConnection conn)
    {
        /*  Specific case: when the name of the room contains '\', then it will introduce a wearied bug */
            conn.ChatRoom = conn.ChatRoom.Replace('\\', '/');
        
        if (ModelState.IsValid)
            return RedirectToAction(nameof(ChatRoom), conn);
        return View(nameof(JoinChat), conn);
    }

    public IActionResult ChatRoom(UserConnection userConnection)
    {
        return View(userConnection);
    }
}