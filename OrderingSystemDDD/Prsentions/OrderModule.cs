using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Ordering.Application.Dtos.CreateOrderDtos;
using Ordering.Application.Order.Commands;
using Ordering.Domain.Sahred;
using Ordering.Infrastructure.BackGroundJobs;
using Quartz.Impl;
using Quartz;
using OrderingSystemDDD.Extinsions;
using System.Reflection.Metadata.Ecma335;

namespace OrderingSystemDDD.Prsentions
{
    public static class OrderModule
    {
        public static void AddOrderEndPoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/Order", async (ISender sender, [FromHeader(Name ="X-Idompotency-Key")]string requestId,OrederCommand createOrder) =>
            {

                // OrederCommand orederCommand = createOrder.Adapt<OrederCommand>();
                var result = await sender.Send(createOrder);
                return result.Match(onSuccess: () => Results.Ok(), onFailure: f => Results.BadRequest(f));
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
