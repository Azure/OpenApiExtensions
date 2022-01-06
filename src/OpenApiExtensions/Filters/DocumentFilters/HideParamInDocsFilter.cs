using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.OpenApiExtensions.Attributes;
using Microsoft.Azure.OpenApiExtensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.DocumentFilters
{
    /// <summary>
    /// A conditional operation filter to hide an API paramater .    
    /// </summary>
    public class HideParamInDocsFilter : IOperationFilter
    {
        private readonly SwaggerConfig _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="HideParamInDocsFilter"/> class.
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
