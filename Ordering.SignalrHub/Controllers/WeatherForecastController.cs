using Microsoft.AspNetCore.Mvc;

namespace Ordering.SignalrHub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WeatherForecast _w ;
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherForecast w)
        {
            _logger = logger;
            _w = w;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task <IEnumerable<WeatherForecast>> Get()
        {
            /* return Enumerable.Range(1, 5).Select(index => new WeatherForecast
             {
                 Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                 TemperatureC = Random.Shared.Next(-20, 55),
                 Summary = Summaries[Random.Shared.Next(Summaries.Length)]
             })
             .ToArray();*/
            await _w.send("zakaria");


            return Enumerable.Empty<WeatherForecast>();
        }
    }
}