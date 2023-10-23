using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Ordering.Application.Dtos.CreateOrderDtos;
using Ordering.Application.Order.Commands;
using Ordering.Domain.Sahred;

namespace OrderingSystemDDD.Prsentions
{
    public static class OrderModule
    {
        public static void AddOrderEndPoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/Order", async (ISender sender, CreateOrderDto createOrder) =>
            {
                var s = new Mapper();
                s.Config.ForType<CreateOrderDto, OrederCommand>().MapToConstructor;
                
                OrederCommand orederCommand = s.Adapt<OrederCommand>();
               var result = await sender.Send(orederCommand);
                if(result != null && result.IsSuccess) {
                    return Results.Ok();//or  TypedResults.Ok(payload)

                }
                else
                {
                    return Results.BadRequest();
                }
                //return Handle(result);
            }).Produces(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status200OK)
               // .AddEndpointFilter<"s"> speacfic filter 
              //.RequireAuthorization()
              .WithName("CreateOrderCommand")
              .WithTags("Order");
        }
      /*  public static IActionResult Handle(Result result) { 

/*return new StatusCodeResult.();*/
                
      //  }



    }
    public class BadRequest : BadRequestResult
    {
        public BadRequest():
            base()
        {

        }
    }

}
