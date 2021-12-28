using System.Net.Mime;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.DocumentFilters
{
    /// <summary>
    /// Adds document level property 'consumes' and sets its value to mime type application/json.
    /// </summary>
    public class AddConsumesFilter : IDocumentFilter
    {
        /// <summary>
        /// Applies filter.
        /// </summary>
        /// <param name="swaggerDoc">OpenApiDocument.</param>
        /// <param name="context">DocumentFilterContext.</param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var consumes = new OpenApiArray();
            consumes.Add(new OpenApiString(MediaTypeNames.Application.Json));
            swaggerDoc.Extensions.Add("consumes", consumes);
        }
    }
}
