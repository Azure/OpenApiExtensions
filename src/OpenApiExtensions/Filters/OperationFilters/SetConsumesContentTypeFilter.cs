using System;
using System.Linq;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.OperationFilters
{
    /// <summary>
    /// Clear all consumed types except application/json.
    /// </summary>
    public class SetConsumesContentTypeFilter : IOperationFilter
    {
        /// <summary>
        /// Applies filter.
        /// </summary>
        /// <param name="operation">OpenApiOperation.</param>
        /// <param name="context">DocumentFilterContext.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.RequestBody != null && operation.RequestBody.Content != null)
            {
                foreach (var content in operation.RequestBody.Content)
                {
                    if (!content.Key.Equals(MediaTypeNames.Application.Json, StringComparison.InvariantCulture))
                    {
                        operation.RequestBody.Content.Remove(content.Key);
                    }
                }

                // Add additional content types passed using ConsumesAttribute
                var consumesAttr = context.ApiDescription.CustomAttributes().OfType<ConsumesAttribute>().FirstOrDefault();
                if (consumesAttr != null)
                {
                    // Assuming json media type will always be present.
                    // Copying its content (for schema object).
                    // TODO throw a exception.
                    var mediaType = operation.RequestBody.Content[MediaTypeNames.Application.Json];

                    // removing default contentType "Application-Json"
                    // if we are adding other contentTypes via attribute in the operation.
                    operation.RequestBody.Content.Remove(MediaTypeNames.Application.Json);

                    foreach (var contentType in consumesAttr.ContentTypes)
                    {
                        operation.RequestBody.Content.Add(contentType, mediaType);
                    }
                }
            }
        }
    }
}