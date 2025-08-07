
using Microsoft.AspNetCore.SignalR;
using System;
namespace Ordering.SignalrHub.Hubs;

public class OrderingHub : Hub
{
   
        // انضمام المستخدم لغرفة طلب محددة
        public async Task JoinOrderRoom(string orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"order-{orderId}");
            Console.WriteLine($"Client {Context.ConnectionId} joined group order-{orderId}");
        }

        // مغادرة غرفة الطلب (اختياري)
        public async Task LeaveOrderRoom(string orderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"order-{orderId}");
            Console.WriteLine($"Client {Context.ConnectionId} left group order-{orderId}");
        }

        // إرسال تحديث للحالة (يمكن استدعاؤها من الـ Controller)
        public async Task NotifyOrderStatusChanged(string orderId, string newStatus)
        {
            await Clients.Group($"order-{orderId}").SendAsync("OrderStatusChanged", newStatus);
        }

    
}
