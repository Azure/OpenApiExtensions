using System;

namespace Microsoft.Azure.OpenApiExtensions.Attributes
{
    /// <summary>
    /// Marked class will be flattened in client library by AutoRest to make it more user freindly.
    /// Level of flatening is governed by using AutoRest argument. default level is 2.
    /// </summary>
    /// <see href="https://github.com/Azure/autorest/tree/master/docs/extensions#x-ms-client-flatten">x-ms-client-flatten.</see>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ClientFlattenAttribute : Attribute
    {
        /// <summary>
        /// Name of the property that will be marked with 'x-ms-client-flatten'.
        /// </summary>
        public string PropertyName { get; set; } = "properties";
    }
}
