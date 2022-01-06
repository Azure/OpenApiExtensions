using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BasicWebAppDemo.WebModels.Common;
using JsonSubTypes;
using Microsoft.Azure.OpenApiExtensions.Attributes;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace BasicWebAppDemo.V1
{
    /// <summary>
    /// Some Descriptive Summary Description (Reflected on your XML Comment -> and Swashbuckle read the XmlDocumentation file and enrich the schamas , see https://github.com/domaindrivendev/Swashbuckle.AspNetCore#include-descriptions-from-xml-comments)
    /// </summary>
    [AzureResource]
    [SwaggerSchema(Required = new[] { "TemperatureC" })]
    [JsonConverter(typeof(JsonSubtypes), "kind")]
    [JsonSubtypes.KnownSubType(typeof(WeatherForecastNetanya), GeoJsonObjectKind.Netanya)]
    [JsonSubtypes.KnownSubType(typeof(WeatherForecastEilat), GeoJsonObjectKind.Eilat)]
    [ClientFlatten]
    public abstract class WeatherForecast
    {
        [Required]
        [JsonProperty(PropertyName = "kind")]
        public abstract GeoJsonObjectKind Kind { get; set; }

        /// <summary>
        /// Get or sets DateTime (this is internal comment and not be shown on swagger, as we use here SwaggerSchema that overrides it )
        /// </summary>
        [SwaggerSchema(Description = "External swagger description")]
        public DateTime Date { get; set; }

        [SwaggerSchema("The WeatherForecast Temperature Celsius", ReadOnly = true)]
        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        [ReadOnlyProperty(false)]
        public string Summary { get; set; }

        /// <summary>
        /// gets or sets props
        /// </summary>
        /// <example>gfgbf</example>
        public SomeObj Properties { get; set; }
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
        public int SomeV1NetanyaProp { get; set; }

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
        public int SomeV1EilatProp { get; set; }

        public override GeoJsonObjectKind Kind { get; set; }
    }
}
