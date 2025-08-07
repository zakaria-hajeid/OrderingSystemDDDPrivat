
using Microsoft.AspNetCore.SignalR;
using System;
namespace Ordering.SignalrHub.Hubs;


    public class chatingHub : Hub
    {
        // تخزين اسم المستخدم مقابل ConnectionId
        private static Dictionary<string, string> UserConnections = new();

        public override Task OnConnectedAsync()
        {
            // ممكن نحط من الكوكي أو query لاحقاً
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            // إزالة المستخدم المنفصل
            var user = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (!string.IsNullOrEmpty(user))
            {
                UserConnections.Remove(user);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public Task RegisterUser(string username)
        {
            // المستخدم يسجّل نفسه باسم عند الاتصال
            if (!UserConnections.ContainsKey(username))
                UserConnections.Add(username, Context.ConnectionId);
            else
                UserConnections[username] = Context.ConnectionId;

            return Task.CompletedTask;
        }

        public async Task SendPrivateMessage(string fromUser, string toUser, string message)
        {
            if (UserConnections.TryGetValue(toUser, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceivePrivateMessage", fromUser, message);
            }
        }
    }



