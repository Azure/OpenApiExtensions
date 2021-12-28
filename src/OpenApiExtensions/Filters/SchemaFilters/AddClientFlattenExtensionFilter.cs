using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Azure.OpenApiExtensions.Attributes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.SchemaFilters
{
    /// <summary>
    /// Marked class will be flattened in client library by AutoRest to make it more user freindly.
    /// Level of flatening is governed by using AutoRest argument. default level is 2.
    /// </summary>
    /// <see href="https://github.com/Azure/autorest/tree/master/docs/extensions#x-ms-client-flatten">x-ms-client-flatten.</see>
    public class AddClientFlattenExtensionFilter : ISchemaFilter
    {
        /// <summary>
        /// Applies filter.
        /// </summary>
        /// <param name="schema">OpenApiSchema.</param>
        /// <param name="context">DocumentFilterContext.</param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties == null || schema.Properties.Count == 0)
            {
                return;
            }

            var flattenAttr = context.Type.GetTypeInfo().GetCustomAttribute<ClientFlattenAttribute>();
            if (flattenAttr != null)
            {
                if (!schema.Properties.ContainsKey(flattenAttr.PropertyName))
                {
                    throw new Exception("Class missing property " + flattenAttr.PropertyName);
                }
                var propertyToAddFlattern = schema.Properties[flattenAttr.PropertyName];
                // in case of $ref exist, swashbuckle will ignore all extensions
                if (propertyToAddFlattern.Reference != null)
                {
                    var origRef = propertyToAddFlattern.Reference;
                    propertyToAddFlattern.Reference = null;
                    propertyToAddFlattern.Extensions.Add("$ref", new OpenApiString(origRef.ExternalResource ?? origRef.ReferenceV2));
                }
                schema.Properties[flattenAttr.PropertyName].Extensions.TryAdd("x-ms-client-flatten", new OpenApiBoolean(true));
            }
        }
    }
}
