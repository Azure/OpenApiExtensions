// <copyright file="HideInDocsFilter.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Filters
{
    using System;
    using System.Linq;
    using System.Reflection;
    using AsiSwaggerExtensions.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Attributes;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// A conditional document filter to hide an API or whole controller.
    /// ** Issue: it is not removing definitions of respective API.
    /// </summary>
    public class HideParamInDocsFilter : IOperationFilter
    {
        private readonly SwaggerConfig _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="HideInDocsFilter"/> class.
        /// </summary>       
        public HideParamInDocsFilter(SwaggerConfig config)
        {
            _config = config;
        }

        /// <inheritdoc/>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (_config.HideParametersEnabled)
            {
                foreach (var parameter in context.MethodInfo.GetParameters())
                {
                    bool isParamHidden = parameter.GetCustomAttributes<SwaggerHideParameterAttribute>().Any();
                    if (isParamHidden)
                    {
                        var name = parameter.GetCustomAttributes<FromHeaderAttribute>()?.FirstOrDefault().Name ??
                            parameter.GetCustomAttributes<FromQueryAttribute>()?.FirstOrDefault().Name ??
                            parameter.Name;

                        var p = operation.Parameters.FirstOrDefault(p => p.Name?.ToLower() == name.ToLower());
                        operation.Parameters.Remove(p);
                    }

                }

            }
        }
    }
}
