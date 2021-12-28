using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Filters.OperationFilters
{
    /// <summary>
    /// The workaround for openapi3/autorest query parameters serialization
    /// For query of type array in parameters, client gen with autorest:  need to specify style propety = "form" so that it doesn't fail.
    /// For swagger 2.0, Items with reference in query parameter is not valid.
    /// This is used to directly add Enums schema to items in case of array type queries instead of references.
    /// This resolves that issue.
    /// See more: https://github.com/Azure/autorest/issues/3373, https://swagger.io/specification/.
    /// Usage guidelines: https://msazure.visualstudio.com/One/_wiki/wikis/AGE%20Agri%20Wiki/95022/ArrayInQueryParametersFilter-usage-guidelines.
    /// </summary>
    public class ArrayInQueryParametersFilter : IOperationFilter
    {
        /// <inheritdoc/>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var parameter in operation?.Parameters.Where(x => (x.In == ParameterLocation.Query && x.Schema.Type == "array")))
            {
                if (!parameter.Style.HasValue)
                {
                    parameter.Style = ParameterStyle.Form;
                    parameter.Explode = true;
                }

                // This is used to directly add Enums to Items instead of reference.
                // For swagger 2.0, Items with reference in query parameter is not valid.
                // This resolves that issue.
                if (parameter.Schema?.Items?.Reference != null &&
                    context?.SchemaRepository?.Schemas.ContainsKey(parameter.Schema.Items.Reference.Id) == true &&
                    context?.SchemaRepository?.Schemas[parameter.Schema.Items.Reference.Id]?.Enum != null)
                {
                    // Explicitly copying only relevant fields to Items.
                    var definition = context.SchemaRepository.Schemas[parameter.Schema?.Items?.Reference.Id];
                    parameter.Schema.Items.Enum = definition.Enum;
                    parameter.Schema.Items.Type = definition.Type;
                    parameter.Schema.Items.Extensions = definition.Extensions;
                    parameter.Schema.Items.Reference = null;
                }
            }
        }
    }
}
