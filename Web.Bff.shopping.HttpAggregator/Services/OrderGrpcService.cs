using GrpcOrder;
using static GrpcOrder.OrderRpc;

namespace Web.Bff.shopping.HttpAggregator.Services
{
    public sealed class OrderGrpcService
    {
        private readonly OrderRpc.OrderRpcClient _OrderRpcClient;

        public OrderGrpcService(OrderRpc.OrderRpcClient orderRpcClient )
        {
            _OrderRpcClient = orderRpcClient;
        }
        public async Task UpdateAsync(object currentBasket)
        {
            var map = new UpdateOrderRequest
            {
                OrderId = "1"
            };

            await _OrderRpcClient.UpdateOrderAsync(map);
        }
    }
}
