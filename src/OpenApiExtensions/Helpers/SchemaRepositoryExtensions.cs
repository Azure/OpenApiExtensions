using System;
using Microsoft.Azure.OpenApiExtensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Azure.OpenApiExtensions.Helpers
{
    public static class SchemaRepositoryExtensions
    {
        public static OpenApiSchema GetOrAdd(this SchemaRepository repo, Type type, string schemaId, Func<OpenApiSchema> factoryMethod)
        {
            if (repo.Schemas.TryGetValue(schemaId, out var schema))
            {
                return schema;
            }
            schema = factoryMethod();
            repo.AddDefinition(schemaId, schema);
            return schema;
        }

        public static bool TryGetIdFor(this SchemaRepository repo, Type type, out string schemaId)
        {
            schemaId = OpenApiOptionsExtension.DefaultSchemaIdSelector(type);
            return repo.TryLookupByType(type, out var schemaOfType);
            //return repo.Schemas.TryGetValue(schemaId, out var _);
        }
    }
}