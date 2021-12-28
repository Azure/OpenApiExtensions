using Microsoft.Azure.OpenApiExtensions.Attributes;

namespace ArmResourceProviderDemo.WebModels.Wind
{
    //[JsonConverter(typeof(TrafficJsonConverter))]
    [SwaggerVirtualInheritances(typeof(WindKindsVirtualInheritanceProvider), nameof(WindResource))]
    public class WindResource : ResourceProxy<WindBaseProperties>, IPropertiesHolder<WindBaseProperties>
    {
    }
}
