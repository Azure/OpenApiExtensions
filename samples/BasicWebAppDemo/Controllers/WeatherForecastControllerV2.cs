using System;
using System.Collections.Generic;
using System.Linq;
using BasicWebAppDemo.V2;
using BasicWebAppDemo.WebModels.Examples;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.OpenApiExtensions.Attributes;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace BasicWebAppDemo.Controllers
{
    [SwaggerTag("Create, read, update and delete WeatherForecast")]
    [ApiController]
    [Route("WeatherForecast")]
    [ApiVersion("2022-01-01-preview")]
    [SwaggerApiVersionRange(fromVersion: "2022-01-01-preview")]
    public class WeatherForecastV2Controller : ControllerBase
    {
        public static IEnumerable<WeatherForecast> Db;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastV2Controller> _logger;

        public WeatherForecastV2Controller(ILogger<WeatherForecastV2Controller> logger)
        {
            _logger = logger;
            var rng = new Random();
            Db = Enumerable.Range(1, 5).Select(index => new WeatherForecastNetanya
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
            Db.ToList().AddRange(
                Enumerable.Range(1, 5).Select(index => new WeatherForecastEilat
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)],
                    SomeV2EilatProp = 33
                }));
        }

        [SwaggerOperation(
            Summary = "Get a weather forecast",
            Description = "fetches from Db",
            OperationId = "Get",
            Tags = new[] { "forecast" })]
        [SwaggerResponse(200, "The WeatherForecast was fetched", typeof(IEnumerable<WeatherForecast>))]
        [SwaggerResponse(400, "invalid request")]
        //[SwaggerResponseExample(200, typeof(StringResponseExample))]
        [ResponseExample(200, typeof(ArrayWeatherForecastExample))]
        [RequestExample(typeof(GetWeatherForecastRequestExample))]
        [HttpGet]
        public IEnumerable<WeatherForecast> GetWeather([FromQuery, SwaggerParameter("WeatherForecast Id", Required = true)] string id)
        {
            return Db;
        }

        [SwaggerOperation(
            Summary = "Post a weather forecast",
            Description = "Post from Db",
            OperationId = "Post",
            Tags = new[] { "forecast" })]
        [SwaggerResponse(200, "The WeatherForecast was fetched", typeof(WeatherForecast))]
        [SwaggerResponse(400, "invalid request")]
        [ResponseExample(200, typeof(WeatherForecastExample))]
        [HttpPost]
        public string PostWeather(V2.WeatherForecast weather)
        {
            return weather.GetType().FullName;
        }

        [SwaggerOperation(
            Summary = "Get a weather forecast",
            Description = "fetches from Db",
            OperationId = "Get_ByGeo",
            Tags = new[] { "forecast" })]
        [SwaggerResponse(200, "The WeatherForecast was fetched", typeof(WeatherForecast))]
        [SwaggerResponse(400, "invalid request")]
        [ResponseExample(200, typeof(WeatherForecastExample))]
        [RequestExample(typeof(GetWeatherForecastRequestExample))]
        [HttpGet("{geo}")]
        public WeatherForecast GetSpecificWeather(string geo, [FromQuery, SwaggerParameter("WeatherForecast Id", Required = true)] string id)
        {
            return Db.First();
        }

        [HideInDocs]
        [SwaggerOperation(
         Summary = "Get a weather forecast",
         Description = "fetches from Db",
         OperationId = "Get",
         Tags = new[] { "forecast" })]
        [SwaggerResponse(200, "The WeatherForecast was fetched", typeof(IEnumerable<WeatherForecast>))]
        [SwaggerResponse(400, "invalid request")]
        //[SwaggerResponseExample(200, typeof(StringResponseExample))]
        [HttpGet("InternalApi")]
        public IEnumerable<WeatherForecast> InternalApi([FromQuery, SwaggerParameter("WeatherForecast Id", Required = true)] string id)
        {
            return Db;
        }
    }
}
