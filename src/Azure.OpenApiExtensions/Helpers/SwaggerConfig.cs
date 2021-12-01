using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OpenApi.Models;

namespace AsiSwaggerExtensions.Helpers
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
        /// Creates shared Definitions for commonly used endpoint parameters (operations parameters) within the swagger document
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <typeparam name="OpenApiParameter"></typeparam>
        /// <returns></returns>
        public Dictionary<string, OpenApiParameter> DocumentLevelReusableParameters { get; set; } = new Dictionary<string, OpenApiParameter>();

        /// <summary>
        /// Gets Or Sets the global parameters, from the Azure spec common library <see cref="https://github.com/Azure/azure-rest-api-specs/tree/main/specification/common-types/resource-management"/>
        /// </summary>
        /// <typeparam name="string">the name of the parameter</typeparam>
        /// <typeparam name="OpenApiParameter">the open Api parameter</typeparam>
        /// <returns></returns>
        public Dictionary<string, OpenApiParameter> GlobalCommonReusableParameters { get; set; } = new Dictionary<string, OpenApiParameter>();

        /// <summary>
        /// Version Level Common Definitions 
        /// </summary>
        /// <typeparam name="string">the Definition (schema) name</typeparam>
        /// <typeparam name="string">the file name where the Definition exists (the file should under the version folder on Common folder)</typeparam>
        /// <returns></returns>
        public Dictionary<string, string> VersionCommonReusableDefinitions { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Resource Provider Common Definitions (refer Common folder under your resource provider <see cref="https://github.com/Azure/azure-rest-api-specs/tree/main/specification/securityinsights/resource-manager/common"/> )
        /// </summary>
        /// <typeparam name="string">the Definition (schema) name </typeparam>
        /// <returns></returns>
        public List<string> ResourceProviderReusableParameters { get; set; } = new List<string>();
        public string XmlCommentFile { get; set; }

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
        public string Title { get; set; } = "Security Insights";
        public string Description { get; set; } = "API spec for Microsoft.SecurityInsights (Azure Security Insights) resource provider";
        public string ClientName { get; set; } = "SecurityInsightsClient";
        public string DefaultVersion { get; set; } = "version1";
        public Dictionary<Type, OpenApiSchema> OverrideMappingTypeToSchema { get; set; } = new Dictionary<Type, OpenApiSchema>();

        public Dictionary<string, OpenApiParameter> GetAllReusableParameters()
        {
            return GlobalCommonReusableParameters
                .Union(DocumentLevelReusableParameters).ToDictionary(k => k.Key, v => v.Value);
        }
    }
}
