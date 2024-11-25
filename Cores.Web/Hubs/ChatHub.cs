using System.Threading.Tasks;
using Cores.Models;
using Microsoft.AspNetCore.SignalR;

namespace Cores.Web.Hubs;

public class ChatHub : Hub
{
    public async Task JoinChat(UserConnection con)
    {
        await Clients.All.SendAsync("ReceiveMessage", "Admin", $"{con.Username} has joined the room");
    }

    public async Task LeaveSpecificRoom(UserConnection con)
    {
        await Clients.Group(con.ChatRoom).SendAsync("ReceiveMessage", "Admin", $"{con.Username} has left the room");
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, con.ChatRoom);
    }

    public async Task JoinSpecificRoom(UserConnection con)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, con.ChatRoom);
        await Clients.Group(con.ChatRoom).SendAsync("ReceiveMessage", "Admin", $"{con.Username} has joined the room");
    }

    public async Task SendMessage(string user, string roomName, string message)
    {
        await Clients.Group(roomName).SendAsync("ReceiveMessage", user, message);
    }
}