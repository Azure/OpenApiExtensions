using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.SchemaFilters
{
    /// <summary>
    /// Converts all reference type to allof types except the onces with polymorphic behavior.
    /// </summary>
    public class ExtendedReferenceSchemaFilter : ISchemaFilter
    {
        private readonly IList<Type> baseTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedReferenceSchemaFilter"/> class.
        /// </summary>
        /// <param name="baseTypes">List of polymorphic base classes.</param>
        public ExtendedReferenceSchemaFilter(IList<Type> baseTypes)
        {
            this.baseTypes = baseTypes;
        }

        /// <inheritdoc/>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties != null)
            {
                foreach (KeyValuePair<string, OpenApiSchema> property in schema.Properties)
                {
                    if (property.Value.AllOf != null && property.Value.AllOf.Count == 1)
                    {
                        if (this.IsReferenceToPolymorphicType(property.Value.AllOf[0]))
                        {
                            property.Value.Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = property.Value.AllOf[0].Reference.Id };
                            property.Value.AllOf = null;
                        }
                    }
                }
            }
        }

        private bool IsReferenceToPolymorphicType(OpenApiSchema schema)
        {
            if (schema.Reference != null)
            {
                foreach (var baseType in this.baseTypes)
                {
                    if (baseType.Name.Equals(schema.Reference.Id, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
