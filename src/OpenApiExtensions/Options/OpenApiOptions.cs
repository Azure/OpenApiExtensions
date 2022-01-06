using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Azure.OpenApiExtensions.Filters;
using Microsoft.Azure.OpenApiExtensions.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.OpenApiExtensions.Filters.OperationFilters;
using Microsoft.Azure.OpenApiExtensions.Filters.SchemaFilters;
using Microsoft.Azure.OpenApiExtensions.Filters.DocumentFilters;

namespace Microsoft.Azure.OpenApiExtensions.Options
{
    public static class OpenApiOptionsExtension
    {
        public static IServiceCollection AddArmCompliantSwagger(this IServiceCollection services, SwaggerConfig config)
        {
            config.EnsureValidity();

            services.AddSwaggerGen(c =>
            {
                IEnumerable<string> actualDocumentsToGenerate = config.SupportedApiVersions;
                if (actualDocumentsToGenerate == null || !actualDocumentsToGenerate.Any())
                {
                    actualDocumentsToGenerate = new[] { config.DefaultApiVersion };
                }
                actualDocumentsToGenerate.ToList().ForEach(v => c.SwaggerDoc(v, OpenApiOptions.GetConfiguration(config, v).GetInfo()));

                // Setting type of IFormFile as "file" in swagger.
                c.MapType(typeof(IFormFile), () => new OpenApiSchema() { Type = "file", Format = "binary" });
                c.MapType(typeof(Stream), () => new OpenApiSchema() { Type = "file", Format = "binary" });
                foreach (var kvp in config.OverrideMappingTypeToSchema)
                {
                    c.MapType(kvp.Key, () => kvp.Value);
                }

                c.CustomOperationIds(d => (d.ActionDescriptor as ControllerActionDescriptor)?.ActionName);

                if (config.UseXmlCommentFiles)
                {
                    var xmlsDocsFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
                    foreach (var xmlCommentFile in xmlsDocsFiles)
                    {
                        c.IncludeXmlComments(xmlCommentFile, true);
                    }
                }

                c.CustomSchemaIds(DefaultSchemaIdSelector);
                c.EnableAnnotations();
                if (!config.GenerateExternalSwagger)
                {
                    //   c.UseInlineDefinitionsForEnums(); // we prefer commonize enums
                }
                if (config.UseAllOfToExtendReferenceSchemas)
                {
                    c.UseAllOfToExtendReferenceSchemas(); // we prefer $ref over AllOf (and set up description ourselves)                    
                }
                c.DocInclusionPredicate((docName, apiDesc) => DocumentApiInclusion(config, docName, apiDesc));

                if (config.GenerateExternalSwagger)
                {
                    c.DocumentFilter<AddHostFilter>(config);
                    c.DocumentFilter<AddSchemesFilter>();
                }
                c.DocumentFilter<AddProducesFilter>();
                c.DocumentFilter<AddConsumesFilter>();

                c.OperationFilter<AddVersionParameterWithExactValueInQuery>();
                c.OperationFilter<ODataParametersSwaggerOperationFilter>();

                c.SchemaFilter<SchemaPropertiesTypesFilter>();
                // Set the body name correctly
                c.RequestBodyFilter<SetBodyNameExtensionFilter>();

                // This is used to remove duplicated api-version query parameter
                c.OperationFilter<RemoveDuplicateApiVersionParameterFilter>();

                // This is used to add the x-ms-long-running-operation attribute and the options
                c.OperationFilter<LongRunningOperationFilter>();

                // OpenAPI capability list documentation: https://msazure.visualstudio.com/One/_git/AGE-Documents?path=%2Fdocs%2FCommon%2FswaggerGenerationLibrary.md&_a=preview
                // Refer above before adding any new SchemaFilter/DocumentFilter, if not present, then can go via adding that in the library and then consuming.
                // Can check individual filers/documents added below for more info.
                c.DocumentFilter<Filters.PolymorphismDocumentFilter>(config.PolymorphicSchemaModels);

                // Schema level filters
                // Works in conjunction with PolymorphismDocumentFilter and PolymorphismSchemaFilter for GeoJsonObject like requirement.
                c.SchemaFilter<SubTypeOfFilter>();

                // Adds 'x-ms-azure-resource' extension to a class marked by Microsoft.Azure.OpenApiExtensions.Attributes.AzureResourceAttribute.
                c.SchemaFilter<AddAzureResourceExtentionFilter>();

                // Manipulates Descriptions of schema mostly used on common generic types
                c.SchemaFilter<CustomSchemaPropertiesFilter>();

                // Sets reusable properties like subscriptionId, resourceGroupName like attributes.
                c.DocumentFilter<SetCustomParametersFilter>(config.GetAllReusableParameters());

                // Sets refs for reusable properties like subscriptionId, resourceGroupName like attributes.
                c.OperationFilter<FormatParametersFilter>(config.GetAllReusableParameters());

                // Adds x-ms-mutability to the property marked by Microsoft.Azure.OpenApiExtensions.Attributes.MutabilityAttribute
                c.SchemaFilter<AddMutabilityExtentionFilter>();

                // Marked class will be flattened in client library by AutoRest to make it more user friendly.
                c.SchemaFilter<AddClientFlattenExtensionFilter>();

                // Adds "readOnly": true to the property marked by Microsoft.Azure.Global.Services.Common.Service.OpenApi.ValidationAttribute.ReadOnlyPropertyAttribute
                c.SchemaFilter<AddReadOnlyPropertyFilter>();

                ////Handle bug that Swashbuckle has: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1488 , https://github.com/Azure/autorest/issues/3417 
                //c.SchemaFilter<ReverseAllOfPropertiesFilter>();
                // Operation level filters
                // Set Description field using the XMLDoc summary if absent and clear summary. By
                // default Swashbuckle uses remarks tag to populate operation description instead
                // of using summary tag.
                c.OperationFilter<MoveSummaryToDescriptionFilter>();

                // Adds x-ms-pageable extension to operation marked with Page-able attribute.
                c.OperationFilter<AddPageableExtensionFilter>();

                // Adds x-ms-long-running-operation extension to operation marked with Microsoft.Azure.OpenApiExtensions.Attributes.LongRunningOperationAttribute.
                c.OperationFilter<AddLongRunningOperationExtensionFilter>();

                c.OperationFilter<DefaultResponseOperationFilter>(config);

                // Clear all the supported mime type from response object. Supported mime type is
                // added at document level, with hard coded value of application/json.
                c.OperationFilter<SetProducesContentTypeFilter>();

                // Clear all consumed types except application/json.
                c.OperationFilter<SetConsumesContentTypeFilter>();

                // Removes parameters that shouldn't be on swagger (if process specifies--externalswagger-gen in command line)
                c.OperationFilter<HideParamInDocsFilter>(config);


                // This is applied if swagger is generated using open api 3.0 spec, helps to fix bug in autorest tool.
                // No impact for swagger generated using 2.0 spec.
                c.OperationFilter<ArrayInQueryParametersFilter>();

                // This is used to set default values, specifically to denote the api version as required parameter.
                c.OperationFilter<SwaggerDefaultValuesFilter>();

                // Adds x-ms-enum to a property enum type. Adds extension attributes to indicate
                // AutoRest to model enum as string. This is as per OpenAPI specifications.
                c.SchemaFilter<AddEnumExtensionFilter>(config.ModelEnumsAsString);

                if (config.GenerateExternalSwagger)
                {
                    c.SchemaFilter<CustomSchemaInheritanceFilter>(config);

                    // This is used to add x-ms-examples field to each operation. We use our own filter in order to allow for customizing the destination path.                    
                    c.DocumentFilter<ExampleFilter>(config);

                    c.DocumentFilter<UpdateCommonRefsDocumentFilter>(config);

                    var conf = OpenApiOptions.GetConfiguration(config);
                    c.AddSecurityRequirement(conf.GetOpenApiSecurityRequirement());
                    c.AddSecurityDefinition("azure_auth", conf.GetAzureSecurityDefinition());
                }
            });
            services.AddSwaggerGenNewtonsoftSupport();
            return services;
        }

        public static IApplicationBuilder UseArmCompliantSwagger(this IApplicationBuilder app, SwaggerConfig swaggerConfig, bool useSwaggerUI = true)
        {
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "swagger/{documentName}/swagger.json";
                // Change generated swagger version to 2.0
                options.SerializeAsV2 = true;
            });
            if (useSwaggerUI)
            {
                app.UseSwaggerUI(option =>
                {
                    IEnumerable<string> actualDocumentsToGenerate = swaggerConfig.SupportedApiVersions;
                    if (actualDocumentsToGenerate == null || !actualDocumentsToGenerate.Any())
                    {
                        actualDocumentsToGenerate = new[] { swaggerConfig.DefaultApiVersion };
                    }
                    actualDocumentsToGenerate.ToList().ForEach(v => option.SwaggerEndpoint($"/swagger/{v}/swagger.json", v));
                });
            }
            return app;
        }


        private static bool DocumentApiInclusion(SwaggerConfig config, string docName, ApiDescription apiDesc)
        {
            return ApiVersionInclusion(config, docName, apiDesc) && VisibilityInclusion(config, docName, apiDesc);
        }

        private static bool VisibilityInclusion(SwaggerConfig config, string docName, ApiDescription apiDesc)
        {
            if (config.GenerateExternalSwagger)
            {
                var apiHideInDocsAttributes = apiDesc.ActionDescriptor.EndpointMetadata.Where(x => x.GetType() == typeof(HideInDocsAttribute)).Cast<HideInDocsAttribute>();
                return !apiHideInDocsAttributes.Any();
            }
            return true;
        }

        private static bool ApiVersionInclusion(SwaggerConfig config, string docName, ApiDescription apiDesc)
        {
            var supportedApiVersions = config.SupportedApiVersions;

            if ((supportedApiVersions == null || !supportedApiVersions.Any()) && !config.GenerateExternalSwagger)
            {
                // all in same version.. 
                return true;
            }
            // this filters apiEndpoints per api-version
            var metadata = apiDesc.ActionDescriptor.EndpointMetadata;
            var apiVersionAttributes = metadata.Where(x => x.GetType() == typeof(ApiVersionAttribute)).Cast<ApiVersionAttribute>();
            var apiVersionRangeAttributes = metadata.Where(x => x.GetType() == typeof(SwaggerApiVersionRangeAttribute)).Cast<SwaggerApiVersionRangeAttribute>();

            var endpointMappedApiVersionsAttributes = metadata.Where(x => x.GetType() == typeof(MapToApiVersionAttribute)).Cast<MapToApiVersionAttribute>();
            if (!apiVersionAttributes.Any() &&
                !endpointMappedApiVersionsAttributes.Any() &&
                !apiVersionRangeAttributes.Any())
            {
                // endpoint is version agnostic, include it on all versions.
                return true;
            }

            var currentDocApiVersion = ApiVersion.Parse(docName);
            if (apiVersionRangeAttributes.Any(range =>
                currentDocApiVersion >= range.FromVersion &&
                (range.ToVersion == null || currentDocApiVersion < range.ToVersion)))
            {
                return true;
            }

            var apiVersions = apiVersionAttributes.SelectMany(x => x.Versions.Select(y => y.ToString())).ToList();
            var endpointVersions = endpointMappedApiVersionsAttributes.SelectMany(x => x.Versions.Select(y => y.ToString())).ToList();

            if (endpointVersions.Any())
            {
                return endpointVersions.Any(v => v == docName);
            }
            return apiVersions.Any(v => v == docName);
        }

        internal static string DefaultSchemaIdSelector(Type modelType)
        {
            // check if generic paramater has custom naming
            if (modelType.IsConstructedGenericType)
            {
                var firstGenericArgType = modelType.GetGenericArguments()[0];
                var applyToParentGenericParamNaming = firstGenericArgType.GetCustomAttributes<SwaggerSchemaNameStrategyAttribute>().SingleOrDefault(at => at.NamingStrategy == NamingStrategy.ApplyToParentWrapper);
                if (applyToParentGenericParamNaming != null)
                {
                    return applyToParentGenericParamNaming.CustomNameProvider.GetCustomName(modelType);
                }
            }

            var customNamingAttribute = modelType.GetCustomAttributes<SwaggerSchemaNameStrategyAttribute>().SingleOrDefault(at => at.NamingStrategy == NamingStrategy.Custom);
            if (customNamingAttribute != null)
            {
                return customNamingAttribute.CustomNameProvider.GetCustomName(modelType);
            }

            if (!modelType.IsConstructedGenericType)
            {
                return modelType.Name;
            }

            var prefix = modelType.GetGenericArguments()
                .Select(genericArg => DefaultSchemaIdSelector(genericArg))
                .Aggregate((previous, current) => previous + current);

            return prefix + modelType.Name.Split('`').First();
        }
    }

    public class OpenApiOptions
    {
        public static Configuration GetConfiguration(SwaggerConfig config, string version = null)
        {
            var info = new OpenApiDocumentInfo(config.Title, config.Description, version ?? config.DefaultApiVersion, config.ClientName);
            return new Configuration(info);
        }
    }
}