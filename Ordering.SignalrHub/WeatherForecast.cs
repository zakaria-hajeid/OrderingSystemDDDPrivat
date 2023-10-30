using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Ordering.SignalrHub.Hubs;

namespace Ordering.SignalrHub
{
    public class WeatherForecast
    {
        public WeatherForecast(IHubContext<NotificationsHub> hubContext)
        {
        }


    }
}
