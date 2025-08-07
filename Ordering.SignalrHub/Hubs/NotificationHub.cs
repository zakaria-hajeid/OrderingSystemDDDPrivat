
using Microsoft.AspNetCore.SignalR;
using System;
namespace Ordering.SignalrHub.Hubs;

public class NotificationsHub : Hub
{
    public override async Task OnConnectedAsync()
    {
      /*كل مستخدم بتعمل باسمو جروب معين وبترسلو هاي الرسائل عالجروب 
       *
       *
       *
       *ليش بعمل مجموعه لليوزر عشان كل كونيكشن الو ممكن يكون من اكثر من جهاز مشان بدل ما ترسلو عجهاز واحد ترسل لكل اجهزتو اللي بالهب 
       */
      
        
        await Groups.AddToGroupAsync(Context.ConnectionId, "zakaria");
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception ex)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
        await base.OnDisconnectedAsync(ex);
    }
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

}
