using System;

namespace Microsoft.Azure.OpenApiExtensions.Attributes
{
    /// <summary>
    /// Model properties marked with this attribute can only be part of response objects and should never be set in request.
    /// </summary>
    public class ReadOnlyPropertyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyPropertyAttribute"/> class.
        /// </summary>
        /// <param name="isReadOnlyProperty">true to show that the property this attribute is bound to is read-only.</param>
        public ReadOnlyPropertyAttribute(bool isReadOnlyProperty)
        {
            IsReadOnlyProperty = isReadOnlyProperty;
        }

        /// <summary>
        /// Is read only property.
        /// </summary>
        public bool IsReadOnlyProperty { get; }
    }
}