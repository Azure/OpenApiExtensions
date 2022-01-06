using Newtonsoft.Json;

namespace Microsoft.Azure.OpenApiExtensions.Helpers
{
    public interface IApiVersionableRequestExample
    {
        public string ApiVersion { get; set; }
    }  
}
