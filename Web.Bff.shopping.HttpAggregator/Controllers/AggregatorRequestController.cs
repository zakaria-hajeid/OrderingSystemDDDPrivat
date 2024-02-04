using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Web.Bff.shopping.HttpAggregator.Services;

namespace Web.Bff.shopping.HttpAggregator.Controllers
{
    [ApiVersion(1)]
    [ApiVersion(2)]
    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    //[Route("api/[controller]")]

    public class AggregatorRequestController : ControllerBase
    {
        private readonly OrderGrpcService orderGrpcService;
        public AggregatorRequestController(OrderGrpcService orderGrpcService)
        {
            this.orderGrpcService = orderGrpcService;
        }
        [HttpPost("Order")]
        [MapToApiVersion(1)]
        public async Task<IActionResult> OrderV1()
        {
            return Ok();
        }
        [HttpPost("Order")]
        [MapToApiVersion(2)]
        public async Task<IActionResult> OrderV2()
        {
            return Ok();
        }
        [HttpPost("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder()
        {
            this.orderGrpcService.UpdateAsync(null);

            return Ok();
        }
    }
}