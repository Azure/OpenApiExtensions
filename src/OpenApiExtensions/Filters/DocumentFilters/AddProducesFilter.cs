using System.Net.Mime;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.DocumentFilters
{
    /// <summary>
    /// Adds document level property 'produce' and sets its value to supported mime type. Only supported type 'application/json'.
    /// </summary>
    public class AddProducesFilter : IDocumentFilter
    {
        /// <summary>
        /// Applies filter.
        /// </summary>
        /// <param name="swaggerDoc">OpenApiDocument.</param>
        /// <param name="context">DocumentFilterContext.</param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var produces = new OpenApiArray();
            produces.Add(new OpenApiString(MediaTypeNames.Application.Json));
            swaggerDoc.Extensions.Add("produces", produces);
        }
    }
}
