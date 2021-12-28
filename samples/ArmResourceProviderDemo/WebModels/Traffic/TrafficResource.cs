using Microsoft.Azure.OpenApiExtensions.Attributes;

namespace ArmResourceProviderDemo.WebModels.Traffic
{
    //[JsonConverter(typeof(TrafficJsonConverter))]
    [SwaggerVirtualInheritances(typeof(TrafficKindsVirtualInheritanceProvider), nameof(TrafficResource))]
    public class TrafficResource : ResourceProxy<TrafficBaseProperties>, IPropertiesHolder<TrafficBaseProperties>
    {
    }
}
