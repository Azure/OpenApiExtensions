using Microsoft.Azure.OpenApiExtensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.DocumentFilters
{
    /// <summary>
    /// Adds document level property host and sets its value to ArmHostName.
    /// </summary>
    public class AddHostFilter : IDocumentFilter
    {
        private SwaggerConfig _config;

        public AddHostFilter(SwaggerConfig config)
        {
            _config = config;
        }
        /// <summary>
        /// Applies filter.
        /// </summary>
        /// <param name="swaggerDoc">OpenApiDocument.</param>
        /// <param name="context">DocumentFilterContext.</param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (!string.IsNullOrEmpty(_config.DefaultHostName))
            {
                swaggerDoc.Extensions.Add("host", new OpenApiString(_config.DefaultHostName));
            }
        }
    }
}
