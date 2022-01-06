using Microsoft.AspNetCore.Mvc;
using System;

namespace Microsoft.Azure.OpenApiExtensions.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class SwaggerApiVersionRangeAttribute : Attribute
    {
        /// <summary>
        /// Range of supported versions
        /// </summary>
        /// <param name="fromVersion">inclusive</param>
        /// <param name="toVersion">non inclusive</param>
        public SwaggerApiVersionRangeAttribute(string fromVersion, string toVersion = null)
        {
            if (fromVersion is null)
            {
                throw new ArgumentNullException(nameof(fromVersion));
            }
            FromVersion = ApiVersion.Parse(fromVersion);
            if (toVersion != null)
            {
                ToVersion = ApiVersion.Parse(toVersion);
            }
        }

        public ApiVersion FromVersion { get; }
        public ApiVersion ToVersion { get; }
    }
}
