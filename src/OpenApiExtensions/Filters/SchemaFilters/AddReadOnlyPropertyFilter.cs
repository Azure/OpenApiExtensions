using System.Linq;
using System.Reflection;
using Microsoft.Azure.OpenApiExtensions.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.SchemaFilters
{
    /// <summary>
    /// Adds "readOnly": true to the property marked by <see cref="ReadOnlyPropertyAttribute"/> in the generated swagger.
    /// </summary>
    public class AddReadOnlyPropertyFilter : ISchemaFilter
    {
        /// <summary>
        /// Applies AddReadOnlyPropertyFilter.
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties == null)
            {
                return;
            }

            var readOnlyAttr = context.Type.GetTypeInfo().GetCustomAttribute<ReadOnlyPropertyAttribute>();
            if (readOnlyAttr != null && readOnlyAttr.IsReadOnlyProperty)
            {
                schema.ReadOnly = true;
            }

            foreach (var schemaProperty in schema.Properties)
            {
                var property = context.Type.GetProperty(schemaProperty.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property != null)
                {
                    var attr = (ReadOnlyPropertyAttribute)property.GetCustomAttributes(typeof(ReadOnlyPropertyAttribute), false).SingleOrDefault();
                    if (attr != null && attr.IsReadOnlyProperty)
                    {
                        schemaProperty.Value.ReadOnly = true;
                    }
                }
            }
        }
    }
}
