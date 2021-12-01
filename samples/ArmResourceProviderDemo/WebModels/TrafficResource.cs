using Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Attributes;

namespace ArmResourceProviderDemo.WebModels
{
    //[JsonConverter(typeof(TrafficJsonConverter))]
    [AsiSwaggerVirtualInheritances(typeof(TrafficKindsVirtualInheritanceProvider), nameof(TrafficResource))]
    public class TrafficResource : ResourceProxy<TrafficBaseProperties>, IPropertiesHolder<TrafficBaseProperties>
    {
    }
    public interface IPropertiesHolder<TPropertiesBase>
    {
        TPropertiesBase Properties { get; set; }
    }
}
