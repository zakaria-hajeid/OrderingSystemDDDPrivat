using EventBus.Abstraction;
using EventBus.Abstraction.RabbitMq;
using EventBus.IntegrationEvents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Services;
using Ordering.Domain.Prematives;
using Ordering.Persistence;
using System.Text.Json;

namespace OrderingSystemDDD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MockPublishController : ControllerBase
    {
        private readonly IEventBus _eventBus;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRabbitMqPublisher<OrderStatusChangedToSubmittedIntegrationEvent> publisher;
        public MockPublishController(IEventBus eventBus, IOrderingIntegrationEventService orderingIntegrationEventService, ApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork, IRabbitMqPublisher<OrderStatusChangedToSubmittedIntegrationEvent> publisher)
        {
            _eventBus = eventBus;
            _orderingIntegrationEventService = orderingIntegrationEventService;
            _applicationDbContext = applicationDbContext;
            _unitOfWork = unitOfWork;
            this.publisher = publisher;
        }
        [Authorize]
        [HttpPost("MockDirictPublish")]
        // mock Reciving end point when ordered paid to set the stok item in repo 
        //From webHook
        public async Task<IActionResult> MockOrderSaveIntegrationEvent()
        {

            var integrationEvent = new OrderStatusChangedToSubmittedIntegrationEvent(1, "sumbuted", "zakaria");


            //test publish 
            // todo try and catch
            publisher.Publish(integrationEvent);

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
        [Authorize]
        [HttpPost("MockRetriveAllIntegrationEventAndPublish")]
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



        [HttpPost("TestApg")]
        // mock Reciving end point when ordered paid to set the stok item in repo 
        //From webHook
        public async Task<IActionResult> TestApg(Guid transactionId)
        {

        
            return Ok();
        }
    }


}
