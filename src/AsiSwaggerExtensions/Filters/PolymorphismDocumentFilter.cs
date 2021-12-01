namespace Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Attributes;
    using Microsoft.OpenApi.Any;
    using Microsoft.OpenApi.Models;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// By default polymorphic base classes are not added in the schema definition of generated swagger.
    /// PolymorphismDocumentFilter initializes schema definitions and register them.
    /// </summary>
    public class PolymorphismDocumentFilter : IDocumentFilter
    {
        private readonly IList<Type> baseTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolymorphismDocumentFilter"/> class.
        /// </summary>
        /// <param name="baseTypes">List of polymorphic base classes.</param>
        public PolymorphismDocumentFilter(IList<Type> baseTypes)
        {
            this.baseTypes = baseTypes;
        }

        /// <inheritdoc/>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (Type abstractType in this.baseTypes)
            {
                RegisterSubClasses(swaggerDoc, context.SchemaRepository, context.SchemaGenerator, abstractType);
            }
        }

        private static void RegisterSubClasses(OpenApiDocument swaggerDoc, SchemaRepository schemaRegistry, ISchemaGenerator schemaGenerator, Type abstractType)
        {
            // Following code can throw an exception. It is expected by the code that first parameter of
            // JsonConverterAttribute.ConverterParameters is set to discriminator name when using
            // JsonConverterAttribute for polymorphic behavior support.
            var converterAttribute = abstractType.GetCustomAttribute<JsonConverterAttribute>();
            //var swaggerVirtualInheritenceAttributes = abstractType.GetCustomAttributes<AsiSwaggerVirtualInheritenceAttribute>();
            var swaggerVirtualInheritanceAttribute = abstractType.GetCustomAttribute<AsiSwaggerVirtualInheritancesAttribute>();

            var customSwaggerSchemaInheritanceAttribute = abstractType.GetCustomAttribute<CustomSwaggerSchemaInheritanceAttribute>();
            string discriminatorName = "kind";
            // get list of all derived classes
            var derivedTypes = abstractType.GetTypeInfo().Assembly.GetTypes()
                .Where(x => abstractType != x && abstractType.IsAssignableFrom(x));

            Dictionary<string, VirtuallyInheritedObjectProperties> virtualInheritances = null;

            if (swaggerVirtualInheritanceAttribute != null)
            {
                discriminatorName = swaggerVirtualInheritanceAttribute.Discriminator;
                virtualInheritances = swaggerVirtualInheritanceAttribute.VirtualInheritancesProvider.GetVirtualInheritances(swaggerDoc.Info.Version);
                derivedTypes = virtualInheritances.Values.Select(v => v.InheritedSubClassType);
            }
            else if (converterAttribute == null || converterAttribute.ConverterParameters == null || converterAttribute.ConverterParameters.Length < 1)
            {
                throw new InvalidOperationException($"Polymorphic Type {abstractType.Name}, must be decorated with JsonConverterAttribute with a valid discriminator, see https://manuc66.github.io/JsonSubTypes/");
            }
            else
            {
                discriminatorName = converterAttribute.ConverterParameters.First().ToString();
            }


            OpenApiSchema schema = null;

            if (schemaRegistry.TryGetIdFor(abstractType, out string schemaId))
            {
                schema = schemaRegistry.Schemas[schemaId];
            }
            else
            {
                schemaGenerator.GenerateSchema(abstractType, schemaRegistry);

                schema = schemaRegistry.Schemas[Helpers.OpenApiOptionsExtension.DefaultSchemaIdSelector(abstractType)];
            }

            // set up a discriminator property (it must be required)
            schema.Discriminator = new OpenApiDiscriminator() { PropertyName = discriminatorName };
            schema.Required = new HashSet<string> { discriminatorName };

            // Throw exception of there is no property present in the class matching discriminator name
            if (customSwaggerSchemaInheritanceAttribute == null && (schema.Properties == null || !schema.Properties.ContainsKey(discriminatorName)))
            {
                throw new Exception("Missing property in " + abstractType.Name + " matching discriminator name");
            }


            // generate and register schema for all of the derived classes
            foreach (var derivedType in derivedTypes)
            {
                schemaGenerator.GenerateSchema(derivedType, schemaRegistry);

                schemaRegistry.TryGetIdFor(derivedType, out string id);
                var derivedSchema = schemaRegistry.Schemas[id];

                if (virtualInheritances != null)
                {
                    var inheritedKvp = virtualInheritances.FirstOrDefault(kvp => kvp.Value.InheritedSubClassType.FullName == derivedType.FullName);
                    if (inheritedKvp.Value != null)
                    {
                        string discriminatorValue = inheritedKvp.Key.ToString();
                        derivedSchema.AllOf = new List<OpenApiSchema> { new OpenApiSchema { Reference = new OpenApiReference { ExternalResource = $"#/definitions/{swaggerVirtualInheritanceAttribute.InheritedFromName}" } } };
                        derivedSchema.Extensions.Add("x-ms-discriminator-value", new OpenApiString(discriminatorValue));
                    }
                }
            }
        }
    }
}