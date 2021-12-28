using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.OperationFilters
{
    /// <summary>
    /// Set Description field using the XMLDoc summary if absent and clear summary.
    /// By default Swachbuckle uses remarks tag to populate operation description instead of using summary tag.
    /// </summary>
    public class MoveSummaryToDescriptionFilter : IOperationFilter
    {
        /// <summary>
        /// Applies filter.
        /// </summary>
        /// <param name="operation">OpenApiOperation.</param>
        /// <param name="context">DocumentFilterContext.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Description = operation.Description ?? operation.Summary;
            operation.Summary = null;
        }
    }
}
