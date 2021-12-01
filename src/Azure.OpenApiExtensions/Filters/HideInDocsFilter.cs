// <copyright file="HideInDocsFilter.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Filters
{
    using System.Linq;
    using System.Reflection;
    using Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Attributes;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// A conditional document filter to hide an API or whole controller.
    /// ** Issue: it is not removing definitions of respective API.
    /// </summary>
    public class HideInDocsFilter : IDocumentFilter
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="HideInDocsFilter"/> class.
        /// </summary>       
        public HideInDocsFilter()
        {
            //this.config = config;
        }

        /// <inheritdoc/>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // This filter is not applicable while are debugging with Swagger-UI
            // And that's reason we are not using standard attribute [ApiExplorerSettings(IgnoreApi = true)]
            //if (!this.config.IsSwaggerSpecGenerationInProgress)
            //{
            //    return;
            //}

            foreach (var apiDescription in context.ApiDescriptions)
            {
                apiDescription.TryGetMethodInfo(out MethodInfo methodInfo);
                bool areAllApisOfControllerHidden = methodInfo.DeclaringType.GetCustomAttributes<HideInDocsAttribute>().Any();
                bool isApiHidden = methodInfo.GetCustomAttributes<HideInDocsAttribute>().Any();
                if (isApiHidden || areAllApisOfControllerHidden)
                {
                    var pathToRemove = "/" + apiDescription.RelativePath.TrimEnd('/');
                    swaggerDoc.Paths.Remove(pathToRemove);
                }
            }
        }
    }
}
