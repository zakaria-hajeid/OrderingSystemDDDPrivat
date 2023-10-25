using IntegrationEventLogEF.Services;
using MediatR;
using Ordering.Application.Abstraction.Messaging;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.Prematives;
using Ordering.Domain.Repository;
using Ordering.Domain.Sahred;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderEntity= Ordering.Domain.AggregatesModel.OrderAggregate.Order;

namespace Ordering.Application.Order.Commands
{
    internal sealed class OrederCommandHandler : ICommandHandler<OrederCommand>
    {
        private readonly IOrderRepository _orderRepository;
        public OrederCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
       // public bool isTransaction => true;//عشان اقرر من البايب لاين اني افتح ترانز اكشن او لا 
        public async Task<Result> Handle(OrederCommand request, CancellationToken cancellationToken)
        {

            //you should clean the basket event 
            var address = Address.CreateAddress(request.Street, request.City, request.State, request.Country, request.ZipCode);
            var order = OrderEntity.CreateOrder(address, request.UserId, request.UserName, request.CardTypeId, request.CardNumber, request.CardSecurityNumber, request.CardHolderName, request.CardExpiration);
            ICollection<Result> AddOrderItem = new List<Result>();
            foreach (var item in request.orderItemDtos)
            {
                AddOrderItem.Add(order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Discount, item.PictureUrl, item.Units));
            }
            if (AddOrderItem.All(x => x.IsSuccess))
            {
                await _orderRepository.Add(order);
                var x = order.Id;
                await _orderRepository.UnitOfWork.PublishEventAsyncAsync();
                return Result.success();
            }
           return Result.Failure<OrederCommand>(AddOrderItem.Select(x=>x.Error).FirstOrDefault()!);
        }
    }
}
