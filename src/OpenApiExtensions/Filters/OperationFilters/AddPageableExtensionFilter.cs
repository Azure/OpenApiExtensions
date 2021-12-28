using System.Linq;
using Microsoft.Azure.OpenApiExtensions.Attributes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.OperationFilters
{
    /// <summary>
    /// Adds x-ms-pageable extension to operation marked with Pageable attribute.
    /// </summary>
    /// <see href="https://github.com/Azure/autorest/tree/master/docs/extensions#x-ms-pageable">x-ms-pageable</see>
    public class AddPageableExtensionFilter : IOperationFilter
    {
        /// <summary>
        /// Applies filter.
        /// </summary>
        /// <param name="operation">OpenApiOperation.</param>
        /// <param name="context">DocumentFilterContext.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var pageAttrs = context.ApiDescription.CustomAttributes().OfType<PageableAttribute>();
            if (pageAttrs.Any())
            {
                var pa = pageAttrs.First();
                var mxPageable = new OpenApiObject();

                if (!string.IsNullOrWhiteSpace(pa.ItemName))
                {
                    mxPageable.Add("itemName", new OpenApiString(pa.ItemName));
                }

                string nextLinkName = "nextLink";
                if (!string.IsNullOrWhiteSpace(pa.NextLinkName))
                {
                    nextLinkName = pa.NextLinkName;
                }

                mxPageable.Add("nextLinkName", new OpenApiString(nextLinkName));

                if (!string.IsNullOrWhiteSpace(pa.OperationName))
                {
                    mxPageable.Add("operationName", new OpenApiString(pa.OperationName));
                }

                operation.Extensions.Add("x-ms-pageable", mxPageable);
            }
        }
    }
}
