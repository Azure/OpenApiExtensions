using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Filters
{
    public class CustomSchemaInheritanceFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {          
            var attr = context.Type.GetCustomAttribute<CustomSwaggerSchemaInheritanceAttribute>();
            if (attr != null && schema.Properties.Any())
            {
                var propsToRemove = schema.Properties
                    .Where(p => !attr.NotInheritedPropertiesName
                        .Select(prop => prop.ToLower())
                        .Contains(p.Key.ToLower()))
                    .ToList();

                foreach (var prop in propsToRemove)
                {
                    var definationToRemove = prop.Value.AllOf?.FirstOrDefault()?.Reference?.Id;
                    if (definationToRemove != null)
                    {
                        // remove AttachedSchemas
                        context.SchemaRepository.Schemas.Remove(definationToRemove);
                    }
                    // remove property as it is inherited from common object
                    schema.Properties.Remove(prop);
                }

                schema.AllOf = new List<OpenApiSchema>
                {
                    new OpenApiSchema
                    {
                        Reference = new OpenApiReference
                        {
                            ExternalResource = GetExternalLink(attr)
                        }
                    }
                };
            }
        }

        private string GetExternalLink(CustomSwaggerSchemaInheritanceAttribute attr)
        {
            switch (attr.DefinitionLevel)
            {
                case CommonObjectType.ResourceProviderCommonDefinition:
                    return $"{UpdateCommonRefsDocumentFilter.RPCommonPrefix}{UpdateCommonRefsDocumentFilter.DefinitionsPrefix}{attr.ExternalSchemaName}";
                case CommonObjectType.GlobalCommonDefinition:
                    return $"{UpdateCommonRefsDocumentFilter.CommonPrefix}{UpdateCommonRefsDocumentFilter.DefinitionsPrefix}{attr.ExternalSchemaName}";
                default:
                    throw new NotSupportedException("Please add more support here...");
            }
        }
    }
}