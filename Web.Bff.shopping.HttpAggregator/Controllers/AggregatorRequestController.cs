using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Web.Bff.shopping.HttpAggregator.Controllers
{
    [ApiVersion(1)]
    [ApiVersion(2)]
    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    //[Route("api/[controller]")]

    public class AggregatorRequestController : ControllerBase
    {
        public AggregatorRequestController()
        {

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
    }
}