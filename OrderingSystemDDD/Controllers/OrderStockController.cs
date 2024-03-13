using EventBus.Abstraction;
using EventBus.IntegrationEvents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Services;
using Ordering.Domain.Prematives;
using Ordering.Persistence;
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
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUnitOfWork _unitOfWork;



        public OrderStockController(ILogger<OrderStockController> logger, IEventBus eventBus, IOrderingIntegrationEventService orderingIntegrationEventService, ApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _eventBus = eventBus;
            _orderingIntegrationEventService = orderingIntegrationEventService;
            _applicationDbContext = applicationDbContext;
            _unitOfWork = unitOfWork;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get(string qquery)
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
            SetOrderStockInput payload = JsonSerializer.Deserialize<SetOrderStockInput>(input.Payload)!;//solve
            return Ok();
        }
       [HttpPost("MockOrderPaidSaveIntegrationEvent")]
        // mock Reciving end point when ordered paid to set the stok item in repo 
        //From webHook
        public async Task<IActionResult> MockOrderSaveIntegrationEvent()
        {

            var integrationEvent = new OrderStatusChangedToSubmittedIntegrationEvent(1, "sumbuted", "zakaria");

            try
            {
                var strategy = _unitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _applicationDbContext.BeginTransactionAsync();

                    await _orderingIntegrationEventService.SaveEventAsync(integrationEvent);
                    await _applicationDbContext.CommitTransactionAsync(transaction);
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ok();
        }
        [HttpPost("MockOrderPaideRetriveIntegrationEvent")]
        // mock Reciving end point when ordered paid to set the stok item in repo 
        //From webHook
        public async Task<IActionResult> MockOrderPaideRetriveIntegrationEvent(Guid transactionId)
        {

            try
            {
                await _orderingIntegrationEventService.PublishEventsThroughEventBusAsync(transactionId); ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ok();
        }
    }



}
public class WebhookData
    {
        public DateTime When { get; set; }

        public string Payload { get; set; }

        public string Type { get; set; }
    }

    public record SetOrderStockInput
{
    public int orderId { get; set; }
    public List<OrderStockItem> stockItems { get; set; }

}
   
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
