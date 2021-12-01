namespace Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Attributes;
    using Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Helpers;
    using Microsoft.OpenApi.Models;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// By default polymorphic base classes are not added in the schema definition of generated swagger.
    /// PolymorphismDocumentFilter initializes schema definitions and register them.
    /// </summary>
    public class SchemaDocumentFilter : IDocumentFilter
    {
        /// <inheritdoc/>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            //swaggerDoc.Components.Schemas.Values.First().
            var types = Assembly.GetEntryAssembly().GetTypes().Where(type => type.GetCustomAttribute<CustomSwaggerSchemaIdAttribute>() != null);
            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<CustomSwaggerSchemaIdAttribute>();
                if (!string.IsNullOrEmpty(attr.ShowOnlyOnDocumentVersion) && attr.ShowOnlyOnDocumentVersion != swaggerDoc.Info.Version)
                {
                    var id = OpenApiOptionsExtension.DefaultSchemaIdSelector(type);
                    context.SchemaRepository.Schemas.Remove(id);
                }
            }
        }
    }
}