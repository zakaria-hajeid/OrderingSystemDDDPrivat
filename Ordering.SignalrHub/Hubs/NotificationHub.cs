
using Microsoft.AspNetCore.SignalR;
namespace Ordering.SignalrHub.Hubs;

public class NotificationsHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        // add the loggin user to specific group name group 
        await Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception ex)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
        await base.OnDisconnectedAsync(ex);
    }
}
