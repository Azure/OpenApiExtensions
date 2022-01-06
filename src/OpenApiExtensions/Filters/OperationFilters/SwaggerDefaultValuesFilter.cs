using System;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.OperationFilters
{
    /// <summary>
    /// Represents the Swagger/Swashbuckle operation filter used to document the implicit API version parameter.
    /// </summary>
    public class SwaggerDefaultValuesFilter : IOperationFilter
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerDefaultValuesFilter"/> class.
        /// </summary>
        public SwaggerDefaultValuesFilter()
        {
        }

        /// <inheritdoc/>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation is null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var apiDescription = context.ApiDescription;

            //operation.Deprecated |= apiDescription.IsDeprecated();

            if (operation.Parameters == null)
            {
                return;
            }

            foreach (var parameter in operation.Parameters.OfType<OpenApiParameter>())
            {
                var description = apiDescription.ParameterDescriptions.FirstOrDefault(p => p.Name == parameter.Name);

                if (description != null)
                {
                    if (parameter.Description == null)
                    {
                        parameter.Description = description.ModelMetadata?.Description;
                    }

                    // Do not populate default value for api-version while specification generation is going on at build time.
                    // while populate for local testing.
                    if (true
                        //&& !this.config.IsSwaggerSpecGenerationInProgress
                        )
                    {
                        if (parameter.Schema.Default == null && description.DefaultValue != null)
                        {
                            parameter.Schema.Default = new OpenApiString(description.DefaultValue.ToString());
                        }
                    }

                    parameter.Required |= description.IsRequired;
                }
            }
        }
    }
}
