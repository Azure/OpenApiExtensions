//-----------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------

namespace Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Filters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mime;
    using Microsoft.Azure.Global.Services.Common.Service.OpenApi.Extensions;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// Clear all the supported mime type from reponse object.
    /// Supported mime type is added at document level, with hardcoded value of application/json.
    /// </summary>
    // Should be extended to support encoding and extension for
    public class SetProducesContentTypeFilter : IOperationFilter
    {
        /// <summary>
        /// Applies filter.
        /// </summary>
        /// <param name="operation">OpenApiOperation.</param>
        /// <param name="context">DocumentFilterContext.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {            
            // Remove all mime types from response except application/json
            foreach (var response in operation.Responses.Values)
            {
                foreach (var contentType in response.Content.Keys)
                {
                    if (!string.Equals(contentType, MediaTypeNames.Application.Json, StringComparison.InvariantCulture))
                    {
                        response.Content.Remove(contentType);
                    }
                }
            }

            // Supporting only one content type of each response type as per OpenApi 2.0 spec.
            var producesAttrs = context.ApiDescription.CustomAttributes().OfType<ProducesContentTypeAttribute>();
            if (producesAttrs.Any())
            {
                // removing default contentType "Application-Json" 
                // if we are adding other contentTypes via attribute in the operation.
                foreach (var response in operation.Responses.Values)
                {
                    response.Content.Remove(MediaTypeNames.Application.Json);
                }

                foreach (var attr in producesAttrs)
                {
                    if (operation.Responses.TryGetValue(
                        attr.StatusCode.ToString(CultureInfo.InvariantCulture), out OpenApiResponse response))
                    {
                        response.Content.Add(attr.ContentType, new OpenApiMediaType());
                    }
                }
            }
        }
    }
}
