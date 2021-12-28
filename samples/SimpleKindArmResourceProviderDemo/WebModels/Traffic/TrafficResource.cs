using Microsoft.Azure.OpenApiExtensions.Attributes;
using Microsoft.Azure.OpenApiExtensions.Helpers;

namespace SimpleKindArmResourceProviderDemo.WebModels.Traffic
{
    [SwaggerVirtualInheritances(typeof(SimpleKindVirtualInheritanceProvider<CountryKind>), typeof(TrafficDetailedItemProvider), typeof(CountryKind), "kind")]
    public class TrafficResource
    {
        public string Id { get; set; }
        public CountryKind kind { get; set; }

        public TrafficBaseProperties properties { get; set; }

    }


    public enum CountryKind
    {
        USA,
        ENGLAND
    }

}
