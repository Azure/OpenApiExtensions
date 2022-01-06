using Microsoft.Azure.OpenApiExtensions.Attributes;

namespace ArmResourceProviderDemo.WebModels.Wind
{
    [SwaggerVirtualInheritances(typeof(WindKindsVirtualInheritanceProvider), nameof(WindResource))]
    public class WindResource : ResourceProxy<WindBaseProperties>, IPropertiesHolder<WindBaseProperties>
    {
    }
}
