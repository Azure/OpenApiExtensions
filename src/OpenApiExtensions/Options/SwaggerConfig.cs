using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using FileNameWithoutExtension = System.String;
using TypeName = System.String;
using ParemeterName = System.String;
using ActualParemeterName = System.String;

#pragma warning disable CS1711 // XML comment has a typeparam tag, but there is no type parameter by that name

namespace Microsoft.Azure.OpenApiExtensions.Options
{
    public class SwaggerConfig
    {
        /// <summary>
        /// Gets Or Sets list of base/abstract types, the Swagger will use register all derived classes as Swagger definitions
        /// make sure to decorate your base classes with JsonConverterAttribute with a valid discriminator (https://manuc66.github.io/JsonSubTypes/)
        /// </summary>
        /// <value></value>
        public List<Type> PolymorphicSchemaModels { get; set; } = new List<Type>();
        public bool ModelEnumsAsString { get; set; } = true;

        /// <summary>
        /// As Siblings of $ref are forbidden, we can use "AllOf" that permits siblings such as description, required, and other
        /// see https://swagger.io/docs/specification/data-models/oneof-anyof-allof-not/#allof
        /// default is "false", so $ref is prefered than AllOf
        /// </summary>
        public bool UseAllOfToExtendReferenceSchemas { get; set; }

        /// <summary>
        /// Creates shared Definitions for commonly used endpoint parameters (operations parameters) within the swagger document
        /// </summary>
        /// <typeparam name="TypeName"></typeparam>
        /// <typeparam name="OpenApiParameter"></typeparam>
        /// <returns></returns>
        public Dictionary<TypeName, OpenApiParameter> DocumentLevelReusableParameters { get; set; } = new Dictionary<TypeName, OpenApiParameter>();

        /// <summary>
        /// Gets Or Sets the global parameters, from the Azure spec common library "https://github.com/Azure/azure-rest-api-specs/tree/main/specification/common-types/resource-management" />
        /// </summary>
        /// <typeparam name="TypeName">the name of the parameter</typeparam>
        /// <typeparam name="OpenApiParameter">the open Api parameter</typeparam>
        /// <returns></returns>
        public Dictionary<TypeName, OpenApiParameter> GlobalCommonReusableParameters { get; set; } = new Dictionary<TypeName, OpenApiParameter>();

        /// <summary>
        /// Version Level Common Definitions 
        /// </summary>
        /// <typeparam name="TypeName">the Definition (schema) name</typeparam>
        /// <typeparam name="FileNameWithoutExtension">the file name where the Definition exists (the file should under the version folder on Common folder)</typeparam>
        /// <returns></returns>
        public Dictionary<TypeName, FileNameWithoutExtension> VersionCommonReusableDefinitions { get; set; } = new Dictionary<TypeName, FileNameWithoutExtension>();


        /// <summary>
        /// Resource Provider Common Definitions (refer Common folder under your resource provider https://github.com/Azure/azure-rest-api-specs/tree/main/specification/securityinsights/resource-manager/common/> )
        /// </summary>
        /// <typeparam name="ParemeterName">the Definition (schema) name </typeparam>
        /// <typeparam name="FileNameWithoutExtension">the Definition (schema) name </typeparam>        
        /// <returns></returns>
        public List<KeyValuePair<ParemeterName, FileNameWithoutExtension>> ResourceProviderReusableParameters { get; set; } = new List<KeyValuePair<ParemeterName, FileNameWithoutExtension>>();

        /// <summary>
        /// Read all XML Documentation files available under the running assembly folder see https://riptutorial.com/csharp/example/5498/generating-xml-from-documentation-comments#:~:text=To%20generate%20an%20XML%20documentation%20file%20from%20documentation,-%3E%20Output%2C%20check%20the%20XML%20documentation%20file%20checkbox%3A"
        /// </summary>
        public bool UseXmlCommentFiles { get; set; } = true;

        /// <summary>
        /// As there are 2 types of Swagger generations, 1 for externing your API to customers, second, for developing API locally
        /// </summary>
        /// <value>true for external, false for local development</value>
        public bool GenerateExternalSwagger { get; set; }

        /// <summary>
        /// This on conjunction with attributes such : SwaggerHideParameterAttribute will hide your Path Parameters from the swagger (consider using with GenerateExternalSwagger=true)
        /// </summary>
        /// <value></value>
        public bool HideParametersEnabled { get; set; }
        public IEnumerable<string> SupportedApiVersions { get; set; }

        /// <see cref="OpenApiDocumentInfo.Title"/>
        public string Title { get; set; }

        /// <see cref="OpenApiDocumentInfo.Description"/>
        public string Description { get; set; }

        /// <see cref="OpenApiDocumentInfo.ClientName"/>
        public string ClientName { get; set; }

        /// <summary>
        /// Defauld Api Verions
        /// </summary>
        public string DefaultApiVersion { get; set; } = "version1";
        public Dictionary<Type, OpenApiSchema> OverrideMappingTypeToSchema { get; set; } = new Dictionary<Type, OpenApiSchema>();

        public string GlobalCommonFilePath { get; set; }
        public string RPCommonFilePath { get; set; }
        public string VersionCommonFolderPath { get; set; }
        public string DefaultHostName { get; set; }

        /// <summary>
        /// Default error Uri path to be added as default response to all operations.
        /// in case of null, no default response will be added
        /// </summary>
        /// <example>../../../../../common-types/resource-management/v3/types.json/#definitions/ErrorResponse</example>
        public string DefaultErrorResponseUri { get; set; }

        public Dictionary<TypeName, OpenApiParameter> GetAllReusableParameters()
        {
            return (GlobalCommonReusableParameters ?? new Dictionary<TypeName, OpenApiParameter>())
                .Union(
                (DocumentLevelReusableParameters ?? new Dictionary<TypeName, OpenApiParameter>()))
                .ToDictionary(k => k.Key, v => v.Value);
        }

        /// <summary>
        /// Validate configuration, and throws if invalid
        /// </summary>
        public void EnsureValidity()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                throw new ArgumentNullException($"Invalid {nameof(SwaggerConfig)} {nameof(Title)} must be set");
            }

            if (string.IsNullOrWhiteSpace(ClientName))
            {
                throw new ArgumentNullException($"Invalid {nameof(SwaggerConfig)} {nameof(ClientName)} must be set");
            }

            if (string.IsNullOrWhiteSpace(Description))
            {
                throw new ArgumentNullException($"Invalid {nameof(SwaggerConfig)} {nameof(Description)} must be set");
            }

            if (GlobalCommonReusableParameters?.Any() == true && string.IsNullOrEmpty(GlobalCommonFilePath))
            {
                throw new InvalidOperationException($"Invalid {nameof(SwaggerConfig)} {nameof(GlobalCommonFilePath)} must be specified, when {nameof(GlobalCommonReusableParameters)} are set");
            }

            if (ResourceProviderReusableParameters?.Any() == true && string.IsNullOrEmpty(RPCommonFilePath))
            {
                throw new InvalidOperationException($"Invalid {nameof(SwaggerConfig)} {nameof(RPCommonFilePath)} must be specified, when {nameof(ResourceProviderReusableParameters)} are set");
            }

            if (VersionCommonReusableDefinitions?.Any() == true && string.IsNullOrEmpty(VersionCommonFolderPath))
            {
                throw new InvalidOperationException($"Invalid {nameof(SwaggerConfig)} {nameof(VersionCommonFolderPath)} must be specified, when {nameof(VersionCommonReusableDefinitions)} are set");
            }
        }
    }
}
