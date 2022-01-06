using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Azure.OpenApiExtensions.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.SchemaFilters
{
    public class CustomSchemaPropertiesFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var attr = context.Type.GetCustomAttribute<CustomSwaggerSchemaAttribute>();
            if (attr != null && !string.IsNullOrEmpty(attr.DescriptionFormat))
            {
                var humanizedCamelCase = string.Empty;
                if (context.Type.GenericTypeArguments != null && context.Type.GenericTypeArguments.Any())
                {
                    humanizedCamelCase = Regex.Replace(context.Type.GenericTypeArguments.First().Name, "([A-Z])", " $1").Trim();
                }
                schema.Description = attr.DescriptionFormat.Replace("{GenericType}", humanizedCamelCase);
            }
        }
    }
}