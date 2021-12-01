using AsiSwaggerExtensions.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Azure.Global.Services.Common.Service.OpenApi.DocumentFilters;
using Microsoft.Azure.Global.Services.Common.Service.OpenApi.OperationFilters;
using Microsoft.Azure.Global.Services.Common.Service.OpenApi.Options;
using Microsoft.Azure.Global.Services.Common.Service.OpenApi.SchemaFilters;
using Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Filters;
using Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Helpers
{
    public static class OpenApiOptionsExtension
    {
        public static IServiceCollection AddArmCompliantSwagger(this IServiceCollection services, SwaggerConfig config)
        {
            services.AddSwaggerGen(c =>
            {
                IEnumerable<string> actualDocumentsToGenerate = config.SupportedApiVersions;
                if (actualDocumentsToGenerate == null || !actualDocumentsToGenerate.Any())
                {
                    actualDocumentsToGenerate = new[] { config.DefaultVersion };
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

                var xmlFile = Path.Combine(AppContext.BaseDirectory, config.XmlCommentFile + ".xml");
                if (!File.Exists(xmlFile))
                {
                    throw new InvalidOperationException(@$"File [{xmlFile}] doesnt exist. 
Documentation file are imporatent enrichment for generating rich swagger files.
make sure to configure documentation file to your WebApi application. The name of the file should be the same as your Assembly Name.
see https://riptutorial.com/csharp/example/5498/generating-xml-from-documentation-comments#:~:text=To%20generate%20an%20XML%20documentation%20file%20from%20documentation,-%3E%20Output%2C%20check%20the%20XML%20documentation%20file%20checkbox%3A");
                }
                c.IncludeXmlComments(xmlFile, true);
                c.CustomSchemaIds(DefaultSchemaIdSelector);
                c.EnableAnnotations();
                if (!config.GenerateExternalSwagger)
                {
                    //   c.UseInlineDefinitionsForEnums(); // we prefer commonize enums
                }
                c.UseAllOfToExtendReferenceSchemas();
                c.DocInclusionPredicate((docName, apiDesc) => ApiVersionInclusion(config, docName, apiDesc));

                if (config.GenerateExternalSwagger)
                {
                    c.DocumentFilter<AddHostFilter>();
                    c.DocumentFilter<AddSchemesFilter>();
                }
                c.DocumentFilter<AddProducesFilter>();
                c.DocumentFilter<AddConsumesFilter>();

                c.OperationFilter<AddVersionParameterWithExactValueInQuery>();
                c.OperationFilter<ODataParametersSwaggerOperationFilter>();
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

                // Adds 'x-ms-azure-resource' extension to a class marked by Microsoft.Azure.Global.Services.Common.Service.OpenApi.Extensions.AzureResourceAttribute.
                c.SchemaFilter<AddAzureResourceExtentionFilter>();

                // Manipulates Descriptions of schema mostly used on common generic types
                c.SchemaFilter<CustomSchemaPropertiesFilter>();

                // Sets reusable properties like subscriptionId, resourceGroupName like attributes.
                c.DocumentFilter<SetCustomParametersFilter>(config.GetAllReusableParameters());

                // Sets refs for reusable properties like subscriptionId, resourceGroupName like attributes.
                c.OperationFilter<Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Filters.FormatParametersFilter>(config.GetAllReusableParameters());

                // Adds x-ms-mutability to the property marked by Microsoft.Azure.Global.Services.Common.Service.OpenApi.Extensions.MutabilityAttribute
                c.SchemaFilter<AddMutabilityExtentionFilter>();

                // Marked class will be flattened in client library by AutoRest to make it more user friendly.
                c.SchemaFilter<AddClientFlattenExtensionFilter>();

                // Adds "readOnly": true to the property marked by Microsoft.Azure.Global.Services.Common.Service.OpenApi.ValidationAttribute.ReadOnlyPropertyAttribute
                c.SchemaFilter<AddReadOnlyPropertyFilter>();

                // PolymorphismSchemaFilter find all the derived classes of passed base classes and register their schema.
                c.SchemaFilter<PolymorphismSchemaFilter>(config.PolymorphicSchemaModels);


                // Operation level filters
                // Set Description field using the XMLDoc summary if absent and clear summary. By
                // default Swashbuckle uses remarks tag to populate operation description instead
                // of using summary tag.
                c.OperationFilter<MoveSummaryToDescriptionFilter>();

                // Adds x-ms-pageable extension to operation marked with Page-able attribute.
                c.OperationFilter<AddPageableExtensionFilter>();

                // Adds x-ms-long-running-operation extension to operation marked with Microsoft.Azure.Global.Services.Common.Service.OpenApi.Extensions.LongRunningOperationAttribute.
                c.OperationFilter<AddLongRunningOperationExtensionFilter>();

                c.OperationFilter<DefaultResponseOperationFilter>(config);

                // Clear all the supported mime type from response object. Supported mime type is
                // added at document level, with hard coded value of application/json.
                c.OperationFilter<Filters.SetProducesContentTypeFilter>();

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
                c.SchemaFilter<Filters.AddEnumExtensionFilter>(config.ModelEnumsAsString);

                if (config.GenerateExternalSwagger)
                {
                    c.SchemaFilter<CustomSchemaInheritanceFilter>();

                    // This is used to skip APIs from documentation conditionally.
                    c.DocumentFilter<HideInDocsFilter>();

                    // This is used to add x-ms-examples field to each operation. We use our own filter in order to allow for customizing the destination path.
                    //c.OperationFilter<AsiExampleFilter>(config);
                    c.DocumentFilter<AsiExampleFilter>(config);

                    c.DocumentFilter<UpdateCommonRefsDocumentFilter>(config);

                    var conf = OpenApiOptions.GetConfiguration(config);
                    c.AddSecurityRequirement(conf.GetOpenApiSecurityRequirement());
                    c.AddSecurityDefinition("azure_auth", conf.GetAzureSecurityDefinition());
                }

                c.DocumentFilter<SchemaDocumentFilter>();

                // Globally enable security based on SwaggerConfig configuration.
                //if (config.EnableSwaggerSecurityTokenSupport)
                //{
                //        c.AddSecurityDefinition(, .GetAzureSecurityDefinition());
                //          ConstantsOpenApiSwagger.AzureAuthSecuritySchemeReferenceId,
                //          ConstantsOpenApiSwagger.AzureAuthOpenApiSecScheme);

                //        // Enable the security definition and requirement for Swagger.
                //        c.AddSecurityRequirement(ConstantsOpenApiSwagger.AzureAuthSecurityRequirement);


                //        if (config.Identity == SwaggerIdentity.ResourceProviderService)
                //    {

                //        c.AddSecurityDefinition(
                //            ConstantsOpenApiSwagger.AzureAuthSecuritySchemeReferenceId,
                //            ConstantsOpenApiSwagger.AzureAuthOpenApiSecScheme);

                //        // Enable the security definition and requirement for Swagger.
                //        c.AddSecurityRequirement(ConstantsOpenApiSwagger.AzureAuthSecurityRequirement);
                //    }
                //    else
                //    {
                //        // First create the security definition for JWT tokens
                //        // The security type "SecuritySchemeType.ApiKey" needs to be used as long as Swagger v2.0 is used
                //        // As soon as Swagger v3.0 is used, the Swagger JSON definition supports the http-scheme with Bearer tokens and will generate correctly.
                //        // More details can be found here:
                //        // - https://swagger.io/docs/specification/2-0/authentication/
                //        // - https://swagger.io/docs/specification/authentication/
                //        c.AddSecurityDefinition(
                //            ConstantsOpenApiSwagger.BearerSecuritySchemeReferenceId,
                //            ConstantsOpenApiSwagger.BearerOpenApiSecScheme);

                //        // Enable the security definition and requirement for Swagger.
                //        c.AddSecurityRequirement(ConstantsOpenApiSwagger.BearerSecurityRequirement);
                //    }


            });
            services.AddSwaggerGenNewtonsoftSupport();
            return services;
        }

        private static bool ApiVersionInclusion(SwaggerConfig config, string docName, ApiDescription apiDesc)
        {
            var supportedApiVersions = config.SupportedApiVersions;
            if (supportedApiVersions == null || !supportedApiVersions.Any())
            {
                // all in same version..
                return true;
            }
            // this filters apiEndpoints per api-version
            var metadata = apiDesc.ActionDescriptor.EndpointMetadata;
            var apiVersionModel = metadata.Where(x => x.GetType() == typeof(ApiVersionAttribute)).Cast<ApiVersionAttribute>();
            var mappedApiVersions = metadata.Where(x => x.GetType() == typeof(MapToApiVersionAttribute)).Cast<MapToApiVersionAttribute>();
            if (!apiVersionModel.Any() && !mappedApiVersions.Any())
            {
                // endpoint is version agnostic, include it on all versions.
                return true;
            }

            var apiVersions = apiVersionModel.SelectMany(x => x.Versions.Select(y => y.ToString())).ToList();
            var versions = mappedApiVersions.SelectMany(x => x.Versions.Select(y => y.ToString())).ToList();

            if (versions.Any())
            {
                return versions.Any(v => v == docName);
            }
            return apiVersions.Any(v => v == docName);
        }

        internal static string DefaultSchemaIdSelector(Type modelType)
        {
            var customNaming = modelType.GetCustomAttribute<CustomSwaggerGenericsSchemaNameStrategy>();

            // check if generic paramater has custom naming
            if (modelType.IsConstructedGenericType)
            {
                var firstGenericArgType = modelType.GetGenericArguments()[0];
                var customGenericParamNaming = firstGenericArgType.GetCustomAttribute<CustomSwaggerGenericsSchemaNameStrategy>();
                if (customGenericParamNaming != null && customGenericParamNaming.NamingStrategy == NamingStrategy.ApplyToParentWrapper)
                {
                    return customGenericParamNaming.CustomNameProvider.GetCustomName(modelType);
                }
            }

            if (customNaming != null && customNaming.NamingStrategy == NamingStrategy.Custom)
            {
                return customNaming.CustomNameProvider.GetCustomName(modelType);
            }

            if (!modelType.IsConstructedGenericType)
            {
                return GetSchemaName(modelType);
            }

            var prefix = modelType.GetGenericArguments()
                .Select(genericArg => DefaultSchemaIdSelector(genericArg))
                .Aggregate((previous, current) => previous + current);

            return prefix + GetSchemaName(modelType).Split('`').First();
        }



        internal static string GetSchemaName(Type type)
        {
            CustomSwaggerSchemaIdAttribute customizeSchemaIdAttribute = (CustomSwaggerSchemaIdAttribute)type.GetCustomAttributes(typeof(CustomSwaggerSchemaIdAttribute), true).FirstOrDefault();
            if (customizeSchemaIdAttribute != null)
            {
                return $"{customizeSchemaIdAttribute.SchemaPrefix}.{ type.Name}";
            }

            //try to find attribute for Schema prefix in type or its parent, otherwise use just the name
            int namespaceDepthLevel = 0;
            var tokens = type.Name.Split(".").TakeLast(namespaceDepthLevel + 1);
            return string.Join(".", tokens);
        }
    }


    public class OpenApiOptions
    {
        public static Configuration GetConfiguration(SwaggerConfig config, string version = null)
        {
            var info = new OpenApiDocumentInfo(config.Title, config.Description, version ?? config.DefaultVersion, config.ClientName);
            return new Configuration(
              info,
              new List<string>(),
              new Dictionary<string, OpenApiParameter>(),
              new List<Type> { });
        }
    }
}