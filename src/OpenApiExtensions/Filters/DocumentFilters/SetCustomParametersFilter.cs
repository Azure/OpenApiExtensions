using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.DocumentFilters
{
    /// <summary>
    /// Populates 'parameter' property with list of all the reusable parameters.
    /// </summary>
    public class SetCustomParametersFilter : IDocumentFilter
    {
        private readonly IDictionary<string, OpenApiParameter> parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetCustomParametersFilter"/> class.
        /// </summary>
        public SetCustomParametersFilter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetCustomParametersFilter"/> class.
        /// </summary>
        /// <param name="parameters">List of resuable parameters.</param>
        public SetCustomParametersFilter(IDictionary<string, OpenApiParameter> parameters)
        {
            this.parameters = parameters;
        }

        /// <summary>
        /// Applies filter.
        /// </summary>
        /// <param name="swaggerDoc">OpenApiDocument.</param>
        /// <param name="context">DocumentFilterContext.</param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (swaggerDoc != null && this.parameters != null && this.parameters.Count > 0)
            {
                swaggerDoc.Components.Parameters = CreateReusableParameters(this.parameters);
            }
        }

        private static Dictionary<string, OpenApiParameter> CreateReusableParameters(IDictionary<string, OpenApiParameter> parameters)
        {
            var commonTypes = new Dictionary<string, OpenApiParameter>() { };

            return commonTypes.Union(parameters).ToDictionary(k => k.Key, v => v.Value);
        }
    }
}
