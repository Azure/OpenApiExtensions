using Newtonsoft.Json;

namespace Microsoft.Azure.OpenApiExtensions.Helpers
{
    public class BasicAsiRequestExample
    {
        public BasicAsiRequestExample()
        {
        }
        public BasicAsiRequestExample(string apiVersion)
        {
            ApiVersion = apiVersion;
        }
        [JsonProperty("api-version")]
        [System.Text.Json.Serialization.JsonPropertyName("api-version")]
        public string ApiVersion { get; set; }
        public string SubscriptionId { get; set; } = "d0cfe6b2-9ac0-4464-9919-dccaee2e48c0";
        public string ResourceGroupName { get; set; } = "myRg";
        public string WorkspaceName { get; set; } = "myWorkspace";
        public string OperationalInsightsResourceProvider { get; set; } = "Microsoft.OperationalInsights";
    }
}
