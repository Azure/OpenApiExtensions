using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Filters
{
    public class AddVersionParameterWithExactValueInQuery : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var existingApiVersion = operation.Parameters.FirstOrDefault(p => p.Name == "api-version");
            if (existingApiVersion == null)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "api-version",
                    In = ParameterLocation.Query,
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        //Default = new OpenApiString(swaggerDoc.Info.Version),
                        //Default = operation.

                        MinLength = 1
                    }
                });
            }
        }
    }
}
