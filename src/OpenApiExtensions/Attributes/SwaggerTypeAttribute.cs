using System;

namespace Microsoft.Azure.OpenApiExtensions.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SwaggerTypeAttribute : Attribute
    {        
        public SwaggerTypeAttribute(string typeName)
        {           
            TypeName = typeName;
        }

        public string TypeName { get; }
    }
}
