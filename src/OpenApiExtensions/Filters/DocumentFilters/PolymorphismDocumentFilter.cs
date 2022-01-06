using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.OpenApiExtensions.Attributes;
using Microsoft.Azure.OpenApiExtensions.Helpers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters
{
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
            foreach (Type abstractType in baseTypes)
            {
                RegisterSubClasses(swaggerDoc, context.SchemaRepository, context.SchemaGenerator, abstractType);
            }
        }

        private void RegisterSubClasses(OpenApiDocument swaggerDoc, SchemaRepository schemaRegistry, ISchemaGenerator schemaGenerator, Type abstractType)
        {
            // Following code can throw an exception. It is expected by the code that first parameter of
            // JsonConverterAttribute.ConverterParameters is set to discriminator name when using
            // JsonConverterAttribute for polymorphic behavior support.
            var converterAttribute = abstractType.GetCustomAttribute<JsonConverterAttribute>();
            //var swaggerVirtualInheritenceAttributes = abstractType.GetCustomAttributes<AsiSwaggerVirtualInheritenceAttribute>();
            var swaggerVirtualInheritanceAttribute = abstractType.GetCustomAttribute<SwaggerVirtualInheritancesAttribute>();

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
                derivedTypes = virtualInheritances.Values.Select(v => v.InheritesClassType == null ? v.InnerPropertyClassType : v.InheritesClassType);
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
                // abstract schema not in this document
                return;
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

                // handle Virtual Inheritance
                if (virtualInheritances != null)
                {
                    var inheritedKvp = virtualInheritances.FirstOrDefault(kvp =>
                    {
                        if (kvp.Value.InheritesClassType != null)
                        {
                            return kvp.Value.InheritesClassType.FullName == derivedType.FullName;
                        }
                        return kvp.Value.InnerPropertyClassType.FullName == derivedType.FullName;
                    });

                    if (inheritedKvp.Value != null)
                    {
                        if (inheritedKvp.Value.InnerPropertyClassType != null)
                        {
                            GenerateInheritanceParent(schemaRegistry, schemaGenerator, swaggerVirtualInheritanceAttribute, inheritedKvp);
                        }
                        else
                        {
                            string discriminatorValue = inheritedKvp.Key.ToString();
                            derivedSchema.AllOf = new List<OpenApiSchema> { new OpenApiSchema { Reference = new OpenApiReference { ExternalResource = $"#/definitions/{swaggerVirtualInheritanceAttribute.InheritedFromName}" } } };
                            derivedSchema.Extensions.Add("x-ms-discriminator-value", new OpenApiString(discriminatorValue));
                        }
                    }
                }
            }
        }

        private void GenerateInheritanceParent(SchemaRepository schemaRegistry, ISchemaGenerator schemaGenerator, SwaggerVirtualInheritancesAttribute swaggerVirtualInheritanceAttribute, KeyValuePair<string, VirtuallyInheritedObjectProperties> derivedType)
        {
            var inheritanceProperties = derivedType.Value;
            var newSchemaName = inheritanceProperties.InheritesClassName;
            if (!schemaRegistry.Schemas.ContainsKey(newSchemaName))
            {
                schemaRegistry.GetOrAdd(GetTempTypeToBind(inheritanceProperties.InnerPropertyClassType), newSchemaName, () =>
                {
                    string discriminatorValue = derivedType.Key.ToString();
                    OpenApiSchema newSchema = new OpenApiSchema();
                    newSchema.Description = inheritanceProperties.InheritesClassDescription?.Length > 0 ? inheritanceProperties.InheritesClassDescription : null;

                    //newSchema.Reference = new OpenApiReference { ExternalResource = $"#/definitions/{swaggerVirtualInheritanceAttribute.InheritedFromName}" };
                    newSchema.AllOf = new List<OpenApiSchema> { new OpenApiSchema { Reference = new OpenApiReference { ExternalResource = $"#/definitions/{swaggerVirtualInheritanceAttribute.InheritedFromName}" } } };
                    newSchema.Properties.Add(inheritanceProperties.InnerPropertyName, BuildInnerProperty(derivedType, newSchemaName));

                    newSchema.Extensions.Add("x-ms-discriminator-value", new OpenApiString(discriminatorValue));
                    newSchema.Type = "object";
                    return newSchema;
                });
            }
        }

        private static OpenApiSchema BuildInnerProperty(KeyValuePair<string, VirtuallyInheritedObjectProperties> derivedType, string newSchemaName)
        {
            OpenApiSchema property = new OpenApiSchema();
            property.Extensions.Add("x-ms-client-flatten", new OpenApiBoolean(true));
            property.Type = "object";

            // workaound: we set $ref as Extension since Reference doesnt allow any other siblings
            property.Extensions.Add("$ref", new OpenApiString($"#/definitions/{derivedType.Value.InnerPropertyClassName}"));
            return property;
        }

        private static Type GetTempTypeToBind(Type type)
        {
            Type concreteTemplateType = typeof(DummyTemplate<>);
            Type uniqueType = concreteTemplateType.MakeGenericType(type);
            return uniqueType;
        }

        class DummyTemplate<TInnerClassType>
        {
        }
    }
}