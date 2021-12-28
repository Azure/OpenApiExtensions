using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.OperationFilters
{
    /// <summary>
    /// Sets id of all the operations to {controller}_{action}
    /// Eg: DerivedModels_ListRelationships.
    /// </summary>
    public class SetOperationIdFilter : IOperationFilter
    {
        /// <summary>
        /// Applies filter.
        /// </summary>
        /// <param name="operation">OpenApiOperation.</param>
        /// <param name="context">DocumentFilterContext.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var opIdAttrs = context.ApiDescription.CustomAttributes().OfType<SwaggerOperationAttribute>();
            if (opIdAttrs.Any(attr => !string.IsNullOrWhiteSpace(attr.OperationId)))
            {
                operation.OperationId = opIdAttrs.First(attr => !string.IsNullOrWhiteSpace(attr.OperationId)).OperationId;
            }
            else
            {
                string controller = context.ApiDescription.ActionDescriptor.RouteValues["controller"];
                string action = context.ApiDescription.ActionDescriptor.RouteValues["action"];
                operation.OperationId = $"{controller}_{action}";
            }
        }
    }
}
