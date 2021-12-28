using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.OpenApiExtensions.Attributes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.SchemaFilters
{
    /// <summary>
    /// Adds x-ms-mutability to the property marked by <see cref="MutabilityAttribute"/>
    /// Offers insight to Autorest on how to generate code (mutability of the property of the model classes being generated).
    /// It doesn't alter the modeling of the actual payload that is sent on the wire.
    /// It is an array of strings with three possible values. The array cannot have repeatable values.
    /// Valid values are: "create", "read", "update".
    /// create: Indicates that the value of the property can be set while creating/initializing/constructing the object
    /// read: Indicates that the value of the property can be read
    /// update: Indicates that value of the property can be updated anytime(even after the object is created).
    /// </summary>
    /// <see href="https://github.com/Azure/autorest/tree/master/docs/extensions#x-ms-mutability">x-ms-mutability.</see>
    public class AddMutabilityExtentionFilter : ISchemaFilter
    {
        /// <summary>
        /// Applies AddMutabilityExtentionFilter.
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var properties = context.Type.GetProperties(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly);
            if (properties == null || properties.Length == 0 || schema.Properties.Count == 0)
            {
                return;
            }

            foreach (var property in properties)
            {
                var mutabilityAttr = property.GetCustomAttribute<MutabilityAttribute>();
                if (mutabilityAttr == null)
                {
                    continue;
                }

                OpenApiArray mutabilities = new OpenApiArray();
                foreach (var mutability in Enum.GetValues(typeof(MutabilityTypes)).Cast<MutabilityTypes>())
                {
                    if ((mutabilityAttr.Mutability & mutability) == mutability)
                    {
                        mutabilities.Add(new OpenApiString(mutability.ToString()));
                    }
                }

                var jsonAttr = property.GetCustomAttribute<JsonPropertyAttribute>();
                if (jsonAttr != null && !string.IsNullOrEmpty(jsonAttr.PropertyName))
                {
                    OpenApiSchema schemaProperty;
                    if (schema.Properties.TryGetValue(jsonAttr.PropertyName, out schemaProperty))
                    {
                        schemaProperty.Extensions.Add("x-ms-mutability", mutabilities);
                    }
                }
                else
                {
                    var name = char.ToLower(property.Name[0], CultureInfo.CurrentCulture) + property.Name.Substring(1);
                    schema.Properties[name].Extensions.Add("x-ms-mutability", mutabilities);
                }
            }
        }
    }
}
