using System.Collections.Generic;
using System.Reflection;
using Microsoft.Azure.OpenApiExtensions.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.SchemaFilters
{
    /// <summary>
    /// Inheritance in OpenApi spec is represented by including all the properties of base class in representation of derived class.
    /// Which leads to repetitions of same fields in all the derived classes representations. To avoid this problem. We have marked
    /// inherited classes with <see cref="SubTypeOfFilter"/>. Using which we will load the base class schemas and remove all base
    /// class properties from derived class.
    /// </summary>
    /// <see href="https://github.com/Azure/azure-rest-api-specs/blob/master/documentation/creating-swagger.md#model-inheritance">model-inheritance.</see>
    public class SubTypeOfFilter : ISchemaFilter
    {
        /// <summary>
        /// Applies filter.
        /// </summary>
        /// <param name="schema">OpenApiSchema.</param>
        /// <param name="context">DocumentFilterContext.</param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var subTypeAttrib = context.Type.GetTypeInfo().GetCustomAttribute<SubTypeOfAttribute>();

            if (subTypeAttrib != null && schema.Type == "object")
            {
                if (schema.AllOf == null)
                {
                    schema.AllOf = new List<OpenApiSchema>();
                }

                // Generate parent schema of current schema and add its reference to current schema
                var schemaRef = context.SchemaGenerator.GenerateSchema(subTypeAttrib.Parent, context.SchemaRepository);
                schema.AllOf.Add(schemaRef);

                // Find all the parent keys to be removed from current schema
                var keysToBeRemoved = new List<string>();
                while (true)
                {
                    // Using id from reference of newly created schema, fetching it from schema repo
                    var parentSchema = context.SchemaRepository.Schemas[schemaRef.Reference.Id];

                    keysToBeRemoved.AddRange(parentSchema.Properties.Keys);

                    // Check if parent schema is also marked with SubTypeAttribute
                    subTypeAttrib = subTypeAttrib.Parent.GetCustomAttribute<SubTypeOfAttribute>();
                    if (subTypeAttrib != null)
                    {
                        schemaRef = context.SchemaGenerator.GenerateSchema(subTypeAttrib.Parent, context.SchemaRepository);
                    }
                    else
                    {
                        break;
                    }
                }

                // Once we have the parent schame, we can remove properties defined by the parent from the child
                foreach (string propName in keysToBeRemoved)
                {
                    schema.Required.Remove(propName);
                    schema.Properties.Remove(propName);
                }
            }
        }
    }
}
