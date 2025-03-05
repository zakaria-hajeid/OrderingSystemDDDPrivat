using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Dtos.CreateOrderDtos;
using Ordering.Application.Order.Commands;
using Ordering.Domain.Sahred;
using OrderingSystemDDD.Extinsions;

namespace OrderingSystemDDD.Prsentions
{
    public static class OrderModule
    {
        public static void AddOrderEndPoints(this IEndpointRouteBuilder app)
        {


            /*
             example of request 
            url :https://localhost:7264/api/Order/Create


            url from apg "10.1.20/Order-api/order/create"

            add header X-Idompotency-Key
            {
  "requestId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userId": "10",
  "userName": "zakaria",
  "city": "amman",
  "street": "jubiha",
  "state": "amman",
  "zipCode": "125",
  "country": "Jordan",
  "cardNumber": "1235",
  "cardHolderName": "zakaria lhajeid",
  "cardExpiration": "2027-01-09T07:39:21.197Z",
  "cardSecurityNumber": "235",
  "cardTypeId": 1,
  "orderItemDtos": [
    {
      "productId": 1,
      "productName": "byclcy",
      "unitPrice": 1,
      "discount": 0,
      "units": 1,
      "pictureUrl": "adsdasdasd"
    }
  ]
}
             */
            app.MapPost("/api/Order/Create", async (ISender sender, [FromHeader(Name ="X-Idompotency-Key")]string requestId, CreateOrderDto createOrder) =>
            {
                OrederCommand orederCommand = createOrder.Adapt<OrederCommand>();
                var result = await sender.Send(orederCommand);
                Result<int> resut = (Result<int>)result;
                return result.Match(onSuccess: () => Results.Ok(resut), onFailure: f => Results.BadRequest(f));
                })
              .Produces(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status200OK)
              // .AddEndpointFilter<"s"> speacfic filter 
              //.RequireAuthorization()
              .WithName("CreateOrderCommand")
              .WithTags("Order");
        }


    }
}
