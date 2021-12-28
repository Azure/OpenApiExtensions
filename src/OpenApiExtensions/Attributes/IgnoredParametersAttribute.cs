using System;
using System.Collections.Generic;

namespace Microsoft.Azure.OpenApiExtensions.Attributes
{
    /// <summary>
    /// Parameters to be ignored from OpenApi spec.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class IgnoredParametersAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IgnoredParametersAttribute"/> class.
        /// </summary>
        /// <param name="parameterNames">List of parameter names.</param>
        public IgnoredParametersAttribute(string[] parameterNames)
        {
            if (parameterNames == null || parameterNames.Length == 0)
            {
                throw new ArgumentNullException(nameof(parameterNames));
            }

            this.ParameterNames = new HashSet<string>(parameterNames);
        }

        /// <summary>
        /// Gets the content types of response.
        /// </summary>
        public HashSet<string> ParameterNames { get; }
    }
}
