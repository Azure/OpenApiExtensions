using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Microsoft.Azure.OpenApiExtensions.Filters.OperationFilters
{
    /// <summary>
    /// Add the supported odata parameters for Odata queries endpoints.
    /// </summary>
    public class ODataParametersSwaggerOperationFilter : Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var odataParam = context.MethodInfo.GetParameters().FirstOrDefault(p => p.ParameterType.BaseType.FullName == "Microsoft.AspNet.OData.Query.ODataQueryOptions");
            if (odataParam != null)
            {
                var p = operation.Parameters.FirstOrDefault(p => p.Name.ToLower() == odataParam.Name.ToLower());
                operation.Parameters.Remove(p);
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$filter",
                    Description = "Filter the results using OData syntax.",
                    Required = false,
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema { Type = "string" }
                });
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$orderby",
                    Description = "Order the results using OData syntax.",
                    Required = false,
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema { Type = "string" }
                });
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$skipToken",
                    Description = "The continuation token for scorlling the items",
                    Required = false,
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema { Type = "string" }
                });
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$top",
                    Description = "The number of results to return.",
                    Required = false,
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema { Type = "integer" }
                });
                // $skip - not supported yet leave as comment
                //operation.Parameters.Add(new OpenApiParameter
                //{
                //    Name = "$skip",
                //    Description = "The number of results to skip.",
                //    Required = false,
                //    In = ParameterLocation.Query,
                //    Schema = new OpenApiSchema { Type = "integer" }
                //});
                // $count - not supported yet leave as comment
                //operation.Parameters.Add(new OpenApiParameter
                //{                    
                //    Name = "$count",
                //    Description = "Return the total count.",
                //    Required = false,
                //    In = ParameterLocation.Query,
                //    Schema = new OpenApiSchema { Type = "boolean" }
                //});
            }
        }
    }
}
