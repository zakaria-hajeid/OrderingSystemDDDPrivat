using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Ordering.SignalrHub.Hubs;

namespace Ordering.SignalrHub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestSignalRNotificationController : ControllerBase
    {
        private readonly IHubContext<NotificationsHub> _hub;

        public TestSignalRNotificationController(IHubContext<NotificationsHub> hub)
        {
            _hub = hub;
        }
        [HttpPost]
        public async Task<IActionResult> NotifyAll([FromQuery] string msg)
        {
            await _hub.Clients.Group("zakaria").SendAsync("ReceiveMessage", "Server", msg);
            return Ok("Message sent to all clients");
        }

    }
}