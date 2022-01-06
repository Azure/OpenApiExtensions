using System;

namespace Microsoft.Azure.OpenApiExtensions.Attributes
{
    /// <summary>
    /// By convention, if a schema has an 'allOf' that references only one other schema, AutoRest
    /// treats this reference as an indication that the allOf schema represents a base type being extended.
    /// </summary>
    /// <see href="https://github.com/Azure/azure-rest-api-specs/blob/master/documentation/creating-swagger.md#model-inheritance">model-inheritance.</see>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SubTypeOfAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubTypeOfAttribute"/> class.
        /// </summary>
        /// <param name="parent"></param>
        public SubTypeOfAttribute(Type parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Gets or sets parent class type.
        /// </summary>
        public Type Parent { get; set; }
    }
}
