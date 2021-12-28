using System;

namespace Microsoft.Azure.OpenApiExtensions.Helpers
{
    public interface ICustomSchemaNameProvider
    {
        string GetCustomName(Type type);
    }
}
