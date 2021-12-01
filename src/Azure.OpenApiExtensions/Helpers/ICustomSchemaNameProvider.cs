using System;

namespace AsiSwaggerExtensions.Helpers
{
    public interface ICustomSchemaNameProvider
    {
        string GetCustomName(Type type);
    }
}
