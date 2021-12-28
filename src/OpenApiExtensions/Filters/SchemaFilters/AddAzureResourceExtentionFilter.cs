using System.Reflection;
using Microsoft.Azure.OpenApiExtensions.Attributes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.SchemaFilters
{
    /// <summary>
    /// Adds 'x-ms-azure-resource' extension to a class marked by <see cref="AzureResourceAttribute" />.
    /// Indicates the class represents is a ARM Resource.
    /// </summary>
    /// <see href="https://github.com/Azure/autorest/tree/master/docs/extensions#x-ms-azure-resource">ARM Resource.</see>
    public class AddAzureResourceExtentionFilter : ISchemaFilter
    {
        /// <summary>
        /// Applies AddAzureResourceExtentionFilter.
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var azureResAttr = context.Type.GetTypeInfo().GetCustomAttribute<AzureResourceAttribute>();
            if (azureResAttr != null)
            {
                schema.Extensions.Add("x-ms-azure-resource", new OpenApiBoolean(true));
            }
        }
    }
}
