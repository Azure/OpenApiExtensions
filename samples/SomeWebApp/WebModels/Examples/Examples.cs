using AsiSwaggerExtensions.Helpers;
using SomeWebApp.V1;
using SomeWebApp.WebModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeWebApp.WebModels.Examples
{
    public class ArrayWeatherForecastExample : BodyExamplesProvider
    {
        protected override object GetBodyExample()
        {
            var rng = new Random();
            return new
            {
                Value = Enumerable.Range(1, 2)
                        .Select(index => new WeatherForecastNetanya
                        {
                            Date = DateTime.Now.AddDays(index),
                            TemperatureC = rng.Next(-20, 55),
                            Summary = "Some summary1",
                            Properties = new SomeObj { MyProperty = 1, MyPropertyStr = "str" }
                        })

            };
        }

    }

    public class WeatherForecastExample : BaseExamplesProvider<WeatherForecastNetanya>
    {
        public override ResponseObj<WeatherForecastNetanya> GetTypedExamples()
        {
            var rng = new Random();
            return new ResponseObj<WeatherForecastNetanya>
            {
                Body = new ResponseObj<WeatherForecastNetanya>.BodyType<WeatherForecastNetanya>
                {
                    Value = new WeatherForecastNetanya
                    {
                        Date = DateTime.Now.AddDays(1),
                        TemperatureC = rng.Next(-20, 55),
                        Summary = "Some summary1",
                        Properties = new SomeObj { MyProperty = 1, MyPropertyStr = "str" }
                    }
                }
            };
        }
    }

    public class GetWeatherForecastRequestExample : IExamplesProvider
    {
        public object GetExample()
        {
            return new { id = "testid" };
        }
    }
}
