using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using BasicWebAppDemo.WebModels.Examples;
using BasicWebAppDemo.V1;
using Microsoft.AspNet.OData.Query;
using Microsoft.Azure.OpenApiExtensions.Attributes;

namespace BasicWebAppDemo.Controllers
{
    [SwaggerTag("Create, read, update and delete WeatherForecast")]
    [ApiController]
    [Route("WeatherForecast")]
    [ApiVersion("2021-09-01-preview")]
    [SwaggerApiVersionRange(fromVersion: "2021-09-01-preview", toVersion: "2022-01-01-preview")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        public static IEnumerable<WeatherForecast> _db;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            var rng = new Random();
            _db = Enumerable.Range(1, 5).Select(index => new WeatherForecastNetanya
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
            _db.ToList().AddRange(

                Enumerable.Range(1, 5).Select(index => new WeatherForecastEilat
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)],
                    SomeV1EilatProp = 33
                }));
        }

        [SwaggerOperation(
            Summary = "Get a weather forecast",
            Description = "fetches from Db",
            OperationId = "Get",
            Tags = new[] { "forecast" })]
        [SwaggerResponse(200, "The WeatherForecast was fetched", typeof(IEnumerable<WeatherForecast>))]
        [SwaggerResponse(400, "invalid request")]
        [ResponseExample(200, typeof(ArrayWeatherForecastExample))]
        [RequestExample(typeof(GetWeatherForecastRequestExample))]
        [Example("myfolder","sometitle")]
        [HttpGet]
        public IEnumerable<WeatherForecast> GetWeather([FromQuery, SwaggerParameter("WeatherForecast Id", Required = true)] string id)
        {
            return _db;
        }

        [SwaggerOperation(
            Summary = "Post a weather forecast",
            Description = "Post from Db",
            OperationId = "Post",
            Tags = new[] { "forecast" })]
        [SwaggerResponse(200, "The WeatherForecast was fetched", typeof(WeatherForecast))]
        [SwaggerResponse(400, "invalid request")]
        //[SwaggerResponseExample(200, typeof(StringResponseExample))]
        [ResponseExample(200, typeof(WeatherForecastExample))]
        [HttpPost]
        public string PostWeather(WeatherForecast weather)
        {
            return weather.GetType().FullName;
        }

        /// <summary>
        /// Get a weather forecast From Summary
        /// </summary>
        /// <param name="someParam" example="someParam Example (not supported be swashbuckle)">Some Param from comments</param>
        /// <param name="geo">Common Param</param>
        /// <param name="id"></param>
        /// <remarks>Som Remarks</remarks>
        /// <returns></returns>
        [SwaggerOperation(
            Summary = "Get a weather forecast",
            Description = "fetches from Db",
            OperationId = "Get_ByGeo",
            Tags = new[] { "forecast" })]
        [SwaggerResponse(200, "The WeatherForecast was fetched", typeof(WeatherForecast))]
        [SwaggerResponse(400, "invalid request")]
        //[SwaggerResponseExample(200, typeof(StringResponseExample))]
        [ResponseExample(200, typeof(WeatherForecastExample))]
        [RequestExample(typeof(GetWeatherForecastRequestExample))]
        [HttpGet("{geo}")]
        public WeatherForecast GetSpecificWeather(string someParam, string geo, [FromQuery, SwaggerParameter("WeatherForecast Id", Required = true)] string id)
        {
            return _db.First();
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
            return _db;
        }

        [HttpGet("test1/{val}")]
        public ODataQueryResponse GetOdataTest(int val, string testStr, ODataQueryOptions<WeatherForecast> options)
        {
            return new ODataQueryResponse { Val = val, TestStr = testStr, Filter = options.Filter.RawValue, OrderBy = options.OrderBy.RawValue, SkipToken = options.SkipToken.RawValue };
        }

    }
}
