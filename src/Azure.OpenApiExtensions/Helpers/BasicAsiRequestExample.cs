using Newtonsoft.Json;

namespace AsiSwaggerExtensions.Helpers
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
        public string subscriptionId { get; set; } = "d0cfe6b2-9ac0-4464-9919-dccaee2e48c0";
        public string resourceGroupName { get; set; } = "myRg";
        public string workspaceName { get; set; } = "myWorkspace";
        public string operationalInsightsResourceProvider { get; set; } = "Microsoft.OperationalInsights";
    }
}
