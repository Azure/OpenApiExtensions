using Microsoft.Azure.OpenApiExtensions.Attributes;
using Microsoft.Azure.OpenApiExtensions.Helpers;
using Newtonsoft.Json;

namespace SimpleKindArmResourceProviderDemo.WebModels.Traffic
{
    [SwaggerVirtualInheritances(typeof(SimpleKindVirtualInheritanceProvider<CountryKind>), typeof(TrafficDetailedItemProvider), typeof(CountryKind), "kind")]
    public class TrafficResource
    {
        public string Id { get; set; }

        [JsonProperty("kind")]
        public CountryKind Kind { get; set; }

        public TrafficBaseProperties Properties { get; set; }
    }

    public enum CountryKind
    {
        USA,
        ENGLAND
    }
}
