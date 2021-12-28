using System.Linq;
using System.Reflection;
using Microsoft.Azure.OpenApiExtensions.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.SchemaFilters
{
    public class SchemaPropertiesTypesFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var props = context.Type.GetProperties();
            foreach (var prop in props)
            {
                var typeAtt = prop.GetCustomAttribute<SwaggerTypeAttribute>();
                if (typeAtt != null)
                {
                    var schemaProp = schema.Properties.FirstOrDefault(kvp => kvp.Key.ToLower() == prop.Name.ToLower());
                    if (schemaProp.Value != null)
                    {
                        schemaProp.Value.Type = typeAtt.TypeName;
                    }
                }
            }
        }
    }
}