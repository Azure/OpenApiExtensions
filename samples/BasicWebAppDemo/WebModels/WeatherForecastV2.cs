using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BasicWebAppDemo.WebModels.Common;
using JsonSubTypes;
using Microsoft.Azure.OpenApiExtensions.Attributes;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace BasicWebAppDemo.V2
{
    [AzureResource]
    [JsonConverter(typeof(JsonSubtypes), "kind")]
    [JsonSubtypes.KnownSubType(typeof(WeatherForecastNetanya), GeoJsonObjectKind.Netanya)]
    [JsonSubtypes.KnownSubType(typeof(WeatherForecastEilat), GeoJsonObjectKind.Eilat)]
    [ClientFlatten]
    public abstract class WeatherForecast
    {
        [Required]
        [JsonProperty(PropertyName = "kind")]
        public abstract GeoJsonObjectKind Kind { get; set; }

        public DateTime Date { get; set; }

        [SwaggerSchema("The WeatherForecast Temperature Celsius", ReadOnly = true)]
        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        [ReadOnlyProperty(false)]
        public string Summary { get; set; }

        /// <summary>
        /// gets or sets props
        /// </summary>
        public SomeObj Properties { get; set; }

        /// <summary>
        /// gets or sets props
        /// </summary>
        public string Version { get; set; } = "2";
    }

    /// <summary>
    /// Som description!
    /// </summary>
    [SubTypeOf(typeof(WeatherForecast))]
    public class WeatherForecastNetanya : WeatherForecast
    {
        public WeatherForecastNetanya()
        {
            Kind = GeoJsonObjectKind.Netanya;
        }

        [Mutability(Mutability = MutabilityTypes.Read)]
        [ReadOnly(true)]
        public int SomeV2NetanyaProp { get; set; }

        public override GeoJsonObjectKind Kind { get; set; }
    }

    [SubTypeOf(typeof(WeatherForecast))]
    public class WeatherForecastEilat : WeatherForecast
    {
        public WeatherForecastEilat()
        {
            Kind = GeoJsonObjectKind.Eilat;
        }

        [ReadOnly(true)]
        public int SomeV2EilatProp { get; set; }

        public override GeoJsonObjectKind Kind { get; set; }
    }
}
