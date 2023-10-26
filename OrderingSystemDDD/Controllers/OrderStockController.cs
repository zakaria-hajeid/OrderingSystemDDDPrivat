using EventBus.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Order.IntegrationEvents;
using System.Text.Json;

namespace OrderingSystemDDD.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderStockController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<OrderStockController> _logger;
        private readonly IEventBus _eventBus;

        public OrderStockController(ILogger<OrderStockController> logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        // mock Reciving end point when ordered paid to set the stok item in repo 
        //From webHook
        [HttpPost("SetStockItem")]

        public async Task<IActionResult> SetStockItem([FromBody]WebhookData input)
        {
            SetOrderStockInput payload = JsonSerializer.Deserialize<SetOrderStockInput>(input.Payload)!;
            return Ok();
        }
        [HttpPost("MockOrderPaidIntegrationEvent")]
        // mock Reciving end point when ordered paid to set the stok item in repo 
        //From webHook
        public async Task<IActionResult> MockOrderPaidIntegrationEvent()
        {

            List<Ordering.Application.Order.IntegrationEvents.OrderStockItem> OrderStockItem = new List<Ordering.Application.Order.IntegrationEvents.OrderStockItem> ();
            OrderStockItem.Add(new Ordering.Application.Order.IntegrationEvents.OrderStockItem(1, 1));

            OrderStatusChangedToPaidIntegrationEvent orderStatusChangedToPaidIntegrationEvent =
                new OrderStatusChangedToPaidIntegrationEvent(431, 381, OrderStockItem);
            await _eventBus.Publish(orderStatusChangedToPaidIntegrationEvent);
            return Ok();
        }

    }



}
public class WebhookData
    {
        public string When { get; set; }

        public string Payload { get; set; }

        public string Type { get; set; }
    }

    public record SetOrderStockInput(int orderId, List<OrderStockItem> stockItems);
   
    public record OrderStockItem
    {
        public int ProductId { get; }
        public int Units { get; }

        public OrderStockItem(int productId, int units)
        {
            ProductId = productId;
            Units = units;
        }
    }
