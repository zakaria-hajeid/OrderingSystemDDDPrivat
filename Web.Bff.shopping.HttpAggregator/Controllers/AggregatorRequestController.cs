using Microsoft.AspNetCore.Mvc;

namespace Web.Bff.shopping.HttpAggregator.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/agg")]
    public class AggregatorRequestController : ControllerBase
    {
        public AggregatorRequestController()
        {

        }
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> OrderV1()
        {
            return Ok();
        }
        [HttpPost]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> OrderV2()
        {
            return Ok();
        }
    }
}