using JsonSubTypes;
using Microsoft.Azure.Global.Services.Common.Service.OpenApi.Extensions;
using Microsoft.Azure.Global.Services.Common.Service.OpenApi.ValidationAttribute;
using Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Attributes;
using Newtonsoft.Json;
using SomeWebApp.WebModels.Common;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SomeWebApp.V2
{
    [AzureResource]
    //[SwaggerSchema(Required = new[] { "TemperatureC" })]
    //[SwaggerSubTypes(typeof(WeatherForecastNetanya), typeof(WeatherForecastJerusalem))]
    //[SwaggerSubType(typeof(WeatherForecastNetanya))]
    //[SwaggerSubType(typeof(WeatherForecastJerusalem))]
    [JsonConverter(typeof(JsonSubtypes), "kind")]
    [JsonSubtypes.KnownSubType(typeof(WeatherForecastNetanya), GeoJsonObjectKind.Netanya)]
    [JsonSubtypes.KnownSubType(typeof(WeatherForecastJerusalem), GeoJsonObjectKind.Jerusalem)]
    [ClientFlatten]
    [CustomSwaggerSchemaId("V20220101Preview", "2022-01-01-preview")]
    //[KnownType(typeof(WeatherForecastJerusalem))]
    //[JsonConverter(typeof(WeatherForecastConverter))]
    abstract public class WeatherForecast
    {

        [Required]
        [JsonProperty(PropertyName = "kind")]
        abstract public GeoJsonObjectKind Kind { get; set; }
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
        [Mutability(Mutability = MutabilityTypes.read)]
        [ReadOnly(true)]
        public int SomeNetanyaProp { get; set; }
        public override GeoJsonObjectKind Kind { get; set; }
    }

    [SubTypeOf(typeof(WeatherForecast))]
    public class WeatherForecastJerusalem : WeatherForecast
    {
        public WeatherForecastJerusalem()
        {
            Kind = GeoJsonObjectKind.Jerusalem;
        }
        [ReadOnly(true)]
        public int SomeJerusalemProp { get; set; }
        public override GeoJsonObjectKind Kind { get; set; }
    }
}
