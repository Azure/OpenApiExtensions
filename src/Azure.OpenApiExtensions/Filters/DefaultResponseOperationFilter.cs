// <copyright file="RemoveDuplicateApiVersionParameterFilter.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Filters
{
    using System;
    using System.Linq;
    using System.Net.Mime;
    using AsiSwaggerExtensions.Helpers;
    using Microsoft.OpenApi.Any;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// By default MVC API versioning library adding "api-version" query parameter to each API.
    /// And it is getting duplicated when there is an additional model binding with
    /// same "api-version" parameter in any API while actually using this parameter.
    ///
    /// This operation filter removes the duplicated "api-version" query parameter.
    /// </summary>
    public class DefaultResponseOperationFilter : IOperationFilter
    {
        private readonly SwaggerConfig _config;

        public DefaultResponseOperationFilter(SwaggerConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <inheritdoc/>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (_config.GenerateExternalSwagger)
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

                var schema = new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Link, ExternalResource = $"{UpdateCommonRefsDocumentFilter.CommonPrefix}{UpdateCommonRefsDocumentFilter.DefinitionsPrefix}ErrorResponse" } };

                defaultErrorResponse.Content.Add(MediaTypeNames.Application.Json, new OpenApiMediaType { Schema = schema });

                operation.Responses.TryAdd("default", defaultErrorResponse);
            }

        }
    }
}
