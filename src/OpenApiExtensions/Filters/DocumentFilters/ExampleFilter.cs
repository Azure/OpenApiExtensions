using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Azure.OpenApiExtensions.Attributes;
using Microsoft.Azure.OpenApiExtensions.Helpers;
using Microsoft.Azure.OpenApiExtensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.DocumentFilters
{
    /// <summary>
    /// Adds x-ms-examples extention to every opertaion. Must run after SetOperationIdFilter.
    /// By default, it will add ./examples/{RelativePath}/{opetationId}.json to all the operations.
    /// Additional examples can be added using <see cref="ExampleAttribute"/>.
    /// Eg: <code>
    /// "x-ms-examples": {
    ///     "DerivedModels_ListBySubscription": {
    ///         "$ref": "./examples/DerivedModels_ListBySubscription.json"
    ///     }
    /// }</code>
    /// </summary>
    /// <see href="https://github.com/Azure/autorest/tree/master/docs/extensions#x-ms-examples">x-ms-examples.</see>
    public class ExampleFilter : IDocumentFilter
    {
        private readonly SwaggerConfig _config;
        public const string ExamplesFolderPath = "./examples/";

        public ExampleFilter(SwaggerConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// Applies filter.
        /// </summary>
        /// <param name="operation">OpenApiOperation.</param>
        /// <param name="apiDescription"></param>
        /// <param name="info"></param>
        public void ApplyWithVersion(OpenApiOperation operation, ApiDescription apiDescription, OpenApiInfo info)
        {
            if (_config.GenerateExternalSwagger)
            {
                var ex = new OpenApiObject();
                var operationPath = operation.Tags?.FirstOrDefault()?.Name ?? ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)apiDescription.ActionDescriptor).ControllerName;

                var defaultOpenApiExampleObject = new OpenApiString($"{ExamplesFolderPath}{operationPath}/{operation.OperationId}.json");
                ex.Add(operation.OperationId, new OpenApiObject { { "$ref", defaultOpenApiExampleObject } });

                var exampleAttributes = apiDescription.CustomAttributes().OfType<ExampleAttribute>();
                if (exampleAttributes.Any())
                {
                    foreach (var example in exampleAttributes)
                    {
                        var explicitExtraOpenApiObject = new OpenApiString($"{ExamplesFolderPath}/{operationPath}/{example.FilePath}.json");
                        ex.Add(example.Title, new OpenApiObject { { "$ref", explicitExtraOpenApiObject } });
                    }
                }

                operation.Extensions.Add("x-ms-examples", ex);

                string projectDirectory = Environment.CurrentDirectory;
                var docName = info.Version;
                string destinationFolder = projectDirectory + $"/Docs/OpenApiSpecs/{docName}/examples/{operationPath}/";

                var outputVarIndex = Array.IndexOf(Environment.GetCommandLineArgs(), "--output");
                if (outputVarIndex >= 0)
                {
                    var outputFile = Environment.GetCommandLineArgs()[outputVarIndex + 1];
                    destinationFolder = Path.Combine(Directory.GetParent(outputFile).FullName, "examples", operationPath);
                }

                string destinationFile = Path.Combine(destinationFolder, operation.OperationId + ".json");

                SwaggerOperationExample exampleObj = new SwaggerOperationExample();
                var responseExampleAttributes = apiDescription.CustomAttributes().OfType<ResponseExampleAttribute>();
                if (responseExampleAttributes.Any())
                {
                    foreach (var example in responseExampleAttributes)
                    {
                        exampleObj.Responses.Add(example.HttpCode.ToString(), example.ExampleProviderInstance.GetExample());
                    }
                }

                var requestExampleAttributes = apiDescription.CustomAttributes().OfType<RequestExampleAttribute>();
                if (requestExampleAttributes.Any())
                {
                    var requestExample = requestExampleAttributes.First().ExampleProviderInstance.GetExample();
                    if ((requestExample as IApiVersionableRequestExample) != null && ((IApiVersionableRequestExample)requestExample).ApiVersion == null)
                    {
                        ((IApiVersionableRequestExample)requestExample).ApiVersion = docName;
                    }
                    exampleObj.Parameters = requestExample;
                }
                var hideAttributes = apiDescription.CustomAttributes().OfType<HideInDocsAttribute>();
                if (!hideAttributes.Any())
                {
                    var jsonSetting = new JsonSerializerSettings() { Formatting = Formatting.Indented, ContractResolver = new CamelCasePropertyNamesContractResolver(), NullValueHandling = NullValueHandling.Ignore };
                    jsonSetting.Converters.Add(new StringEnumConverter());
                    var serialized = JsonConvert.SerializeObject(exampleObj, jsonSetting);
                    Directory.CreateDirectory(destinationFolder);
                    File.WriteAllText(destinationFile, serialized);
                }
            }
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (swaggerDoc == null)
                throw new ArgumentNullException(nameof(swaggerDoc));

            foreach ((string key, OpenApiPathItem value) in swaggerDoc.Paths)
            {
                foreach ((OperationType opType, OpenApiOperation operation) in value.Operations)
                {
                    var apiDescription = context.ApiDescriptions.Single(a => a.HttpMethod.ToLower() == opType.ToString().ToLower() && a.RelativePath.ToLower().Trim('/') == key.ToLower().Trim('/'));
                    ApplyWithVersion(operation, apiDescription, swaggerDoc.Info);
                }
            }

        }

        public class SwaggerOperationExample
        {
            public SwaggerOperationExample()
            {
                Parameters = new object();
                Responses = new Dictionary<string, object>();

            }
            public object Parameters { get; set; }
            public Dictionary<string, object> Responses { get; set; }
        }
    }
}

