using System;

namespace Microsoft.Azure.OpenApiExtensions.Attributes
{
    /// <summary>
    /// Three possible mutability values of a property.
    /// </summary>
    [Flags]
    public enum MutabilityTypes
    {
        /// <summary>
        /// Indicates that the value of the property can be read.
        /// </summary>
        Read = 0x0,

        /// <summary>
        /// Indicates that the value of the property can be set while creating/initializing/constructing the object.
        /// </summary>
        Create = 0x1,

        /// <summary>
        /// Indicates that value of the property can be updated anytime(even after the object is created).
        /// </summary>
        Update = 0x2,
    }

    /// <summary>
    /// Offers insight to Autorest on how to generate code (mutability of the property of the model classes being generated).
    /// It doesn't alter the modeling of the actual payload that is sent on the wire.
    /// It is an array of strings with three possible values. The array cannot have repeatable values.
    /// Valid values are: "create", "read", "update".
    /// create: Indicates that the value of the property can be set while creating/initializing/constructing the object
    /// read: Indicates that the value of the property can be read
    /// update: Indicates that value of the property can be updated anytime(even after the object is created)
    /// </summary>
    /// <see href="https://github.com/Azure/autorest/tree/master/docs/extensions#x-ms-mutability">x-ms-mutability.</see>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MutabilityAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets mutablility of a field Eg: [Mutable(Mutability = MutabilityTypes.create | MutabilityTypes.read)].
        /// </summary>
        public MutabilityTypes Mutability { get; set; } = MutabilityTypes.Create | MutabilityTypes.Read | MutabilityTypes.Update;
    }
}
