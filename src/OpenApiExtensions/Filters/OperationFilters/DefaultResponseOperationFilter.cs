using System;
using System.Net.Mime;
using Microsoft.Azure.OpenApiExtensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.OperationFilters
{
    /// <summary>
    /// adds default response to each Operation
    /// </summary>
    public class DefaultResponseOperationFilter : IOperationFilter
    {
        private readonly SwaggerConfig _config;

        /// <summary>
        /// constracts Default Response filter
        /// </summary>
        /// <param name="config"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DefaultResponseOperationFilter(SwaggerConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <inheritdoc/>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (_config.GenerateExternalSwagger && !string.IsNullOrEmpty(_config.DefaultErrorResponseUri))
            {
                if (operation is null)
                {
                    throw new ArgumentNullException(nameof(operation));
                }

                if (context is null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                var defaultErrorResponse = new OpenApiResponse
                {
                    Description = "Error response describing why the operation failed.",
                };
                defaultErrorResponse.Content.Clear();

                var schema = new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Link, ExternalResource = _config.DefaultErrorResponseUri } };

                defaultErrorResponse.Content.Add(MediaTypeNames.Application.Json, new OpenApiMediaType { Schema = schema });

                operation.Responses.TryAdd("default", defaultErrorResponse);
            }
        }
    }
}
