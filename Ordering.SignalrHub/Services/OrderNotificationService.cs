using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Ordering.SignalrHub.Services
{
    public interface IOrderNotificationService
    {
        Task NotifyOrderStatusChanged(string orderId, string newStatus);
    }

    public class OrderNotificationService : IOrderNotificationService
    {
        private readonly IHubContext<Hubs.OrderingHub> _hubContext;
        public OrderNotificationService(IHubContext<Hubs.OrderingHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task NotifyOrderStatusChanged(string orderId, string newStatus)
        {
            await _hubContext.Clients.Group($"order-{orderId}").SendAsync("OrderStatusChanged", newStatus);
        }
    }
}
