using Grpc.Core;

namespace GrpcOrder
{
    public class OrderService: OrderRpc.OrderRpcBase
    {
        public OrderService()
        {
                
        }
        public override async Task<UpdateOrderRequestResponse> UpdateOrder(UpdateOrderRequest request, ServerCallContext context)
        {

           /* var customerBasket = MapToCustomerBasket(request);

            var response = await _repository.UpdateBasketAsync(customerBasket);

            if (response != null)
            {
                return MapToCustomerBasketResponse(response);
            }

            context.Status = new Status(StatusCode.NotFound, $"Basket with buyer id {request.Buyerid} do not exist");
           */
            return null;
        }
    }
}
    