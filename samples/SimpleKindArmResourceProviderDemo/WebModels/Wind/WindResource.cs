using Microsoft.Azure.OpenApiExtensions.Attributes;
using Microsoft.Azure.OpenApiExtensions.Helpers;
using SimpleKindArmResourceProviderDemo.WebModels.Traffic;

namespace SimpleKindArmResourceProviderDemo.WebModels.Wind
{
    [SwaggerVirtualInheritances(typeof(SimpleKindVirtualInheritanceProvider<CountryKind>), typeof(WindDetailedItemProvider), typeof(CountryKind), "kind")]
    public class WindResource : ResourceProxy<WindBaseProperties>, IPropertiesHolder<WindBaseProperties>
    {
    }
}
