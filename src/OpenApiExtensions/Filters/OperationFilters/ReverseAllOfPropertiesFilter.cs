using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Microsoft.Azure.OpenApiExtensions.Filters.OperationFilters
{
    /// <summary>
    /// Solve known issues with Swashbuckle 5
    /// For more details:
    ///     https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1488 ,
    ///     https://github.com/Azure/autorest/issues/3417
    /// </summary>
    public class ReverseAllOfPropertiesFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            (string key, OpenApiSchema typeSchema)[] allOfProperties = schema.Properties
                .Where(x => null != x.Value.AllOf && 1 == x.Value.AllOf.Count)
                .Select(x => (x.Key, x.Value.AllOf.First()))
                .ToArray();

            foreach ((string key, OpenApiSchema typeSchema) item in allOfProperties)
            {
                schema.Properties[item.key] = item.typeSchema;
            }
        }
    }
}
