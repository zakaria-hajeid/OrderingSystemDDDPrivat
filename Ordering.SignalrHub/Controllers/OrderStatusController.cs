using Microsoft.AspNetCore.Mvc;
using Ordering.SignalrHub.Services;
using System.Threading.Tasks;

namespace Ordering.SignalrHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderStatusController : ControllerBase
    {
        private readonly IOrderNotificationService _orderNotificationService;

        public OrderStatusController(IOrderNotificationService orderNotificationService)
        {
            _orderNotificationService = orderNotificationService;
        }

        [HttpPost("notify")]
        public async Task<IActionResult> NotifyOrderStatusChanged([FromQuery] string orderId, [FromQuery] string newStatus)
        {
            await _orderNotificationService.NotifyOrderStatusChanged(orderId, newStatus);
            return Ok(new { message = "Notification sent." });
        }
    }
}
