using Microsoft.Azure.OpenApiExtensions.Attributes;
//using System.Text.Json.Serialization;

namespace ArmResourceProviderDemo.WebModels.Traffic
{
    //[JsonConverter(typeof(TrafficJsonConverter))]
    [SwaggerVirtualInheritances(typeof(TrafficKindsVirtualInheritanceProvider), nameof(TrafficResource))]
    public class TrafficResource : ResourceProxy<TrafficBaseProperties>, IPropertiesHolder<TrafficBaseProperties>
    {
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public new TrafficKind Kind { get; set; }
    }

    public enum TrafficKind
    {
        India,
        Israel
    }
}
