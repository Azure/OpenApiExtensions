using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.OpenApiExtensions.Attributes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.OperationFilters
{
    /// <summary>
    /// Updates parameter value with references to corresponding document level reusable parameters.
    /// Parameter name is used to identify reusable parameter reference. If not matching name is found in
    /// reusable parameter list, parameter is included as it is.
    /// </summary>
    public class FormatParametersFilter : IOperationFilter
    {
        // Ignored parameters is a exhaustive list of parameters always passed by ARM layer
        // and need not be exposed in OpenApi spec. Some other parameters which can be always ignored can be added here.
        private static readonly HashSet<string> IgnoredParameters = new HashSet<string> { "x-ms-client-tenant-id" };

        private readonly IDictionary<string, OpenApiParameter> parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatParametersFilter"/> class.
        /// </summary>
        public FormatParametersFilter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatParametersFilter"/> class.
        /// </summary>
        /// <param name="parameters">List of reusable parameters.</param>
        public FormatParametersFilter(IDictionary<string, OpenApiParameter> parameters)
        {
            this.parameters = parameters;
        }

        /// <summary>
        /// Applies filter.
        /// </summary>
        /// <param name="operation">OpenApiOperation.</param>
        /// <param name="context">DocumentFilterContext.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var referencesByName = GetParameterReferenceByName();

            var ignoredParamAttrs = context?.ApiDescription.CustomAttributes().OfType<IgnoredParametersAttribute>().FirstOrDefault();
            IList<OpenApiParameter> newParameters = new List<OpenApiParameter>();
            foreach (var parameter in operation?.Parameters)
            {
                if (IgnoredParameters.Contains(parameter.Name) || (ignoredParamAttrs != null && ignoredParamAttrs.ParameterNames.Contains(parameter.Name)))
                {
                    continue;
                }

                if (referencesByName.TryGetValue(parameter.Name, out OpenApiParameter referenceParameter))
                {
                    newParameters.Add(referenceParameter);
                }
                else
                {
                    newParameters.Add(parameter);
                }
            }

            // move Requestbody parameter to Paramaters - to support comlex objects as reusable params
            if (operation.RequestBody != null)
            {
                if (operation.RequestBody.Extensions.TryGetValue("x-bodyName", out var bodyParamName) &&
                    (bodyParamName as OpenApiString) != null &&
                    referencesByName.ContainsKey((bodyParamName as OpenApiString).Value)) // we rely that SetBodyNameExtensionFilter runs first
                {
                    newParameters.Add(referencesByName[(bodyParamName as OpenApiString).Value]);
                    operation.RequestBody = null;
                }

            }
            operation.Parameters = newParameters;
        }

        /// <summary>
        /// Dictionary of ARM reusable parameter references + service specific reusable parameters references if any.
        /// Key: Parameter name, Value: Parameter reference object.
        /// </summary>
        /// <returns>Dictionary of reusable paramters references.</returns>
        public IDictionary<string, OpenApiParameter> GetParameterReferenceByName()
        {
            var parameterReferences = new Dictionary<string, OpenApiParameter>();

            foreach (KeyValuePair<string, OpenApiParameter> keyValuePair in parameters)
            {
                parameterReferences.Add(
                    keyValuePair.Value.Name,
                    new OpenApiParameter
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.Parameter, Id = keyValuePair.Key },
                    });
            }

            return parameterReferences;
        }
    }
}
