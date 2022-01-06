using Microsoft.Azure.OpenApiExtensions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.OpenApiExtensions.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SwaggerVirtualInheritancesAttribute : Attribute
    {
        /// <param name="VirtualInheritancesProviderType">Type of Provider for inheritance. Must Implement IVirtualInheritancesProvider</param>
        /// <param name="InheritedFromName">Name of the parent object</param>
        /// <param name="Discriminator">Field of the parent object that discrimante between different values. Default is "kind"</param>
        public SwaggerVirtualInheritancesAttribute(Type VirtualInheritancesProviderType, string InheritedFromName, string Discriminator = "kind")
        {
            if (VirtualInheritancesProviderType is null)
            {
                throw new ArgumentNullException(nameof(VirtualInheritancesProviderType));
            }
            if (!typeof(IVirtualInheritancesProvider).IsAssignableFrom(VirtualInheritancesProviderType))
            {
                throw new ArgumentException($"argument must be assignable from {nameof(IVirtualInheritancesProvider)}, and must own a valid {nameof(IVirtualInheritancesProvider.GetVirtualInheritances)} method", nameof(VirtualInheritancesProviderType));
            }

            VirtualInheritancesProvider = (IVirtualInheritancesProvider)Activator.CreateInstance(VirtualInheritancesProviderType);
            this.InheritedFromName = InheritedFromName;
            this.Discriminator = Discriminator;
        }

        /// <param name="simpleKindVirtualInheritanceProvider">Type of Provider for inheritance. Must Implement SimpleKindVirtualInheritanceProvider</param>
        /// <param name="virtuallyInheritedItemDetails">Type for an object that provide the child attributes per each discrimantor</param>
        /// <param name="enumType">Enum represent the discrimantor. simpleKindVirtualInheritanceProvider and virtuallyInheritedItemDetails get the enumType as generics</param>
        /// <param name="Discriminator">Field of the parent object that discrimante between different values. Default is "kind"</param>
        public SwaggerVirtualInheritancesAttribute(Type simpleKindVirtualInheritanceProvider, Type virtuallyInheritedItemDetails, Type enumType, string Discriminator = "kind")
        {
            if (enumType is null || !enumType.IsEnum)
            {
                throw new ArgumentNullException(nameof(enumType));
            }

            ValidationUtils.ValidateIsAssignableToGenericType(typeof(SimpleKindVirtualInheritanceProvider<>), enumType, simpleKindVirtualInheritanceProvider);

            ValidationUtils.ValidateIsAssignableToGenericType(typeof(IVirtuallyInheritedItemDetails<>), enumType, virtuallyInheritedItemDetails);

            var virtuallyInheritedItemDetailsInstance = Activator.CreateInstance(virtuallyInheritedItemDetails);
            //var bla = new SimpleKindVirtualInheritanceProvider<CountryKind>(typeof(CountryKind), new WindDetailedItemProvider());
            VirtualInheritancesProvider = (IVirtualInheritancesProvider)Activator.CreateInstance(simpleKindVirtualInheritanceProvider, enumType, virtuallyInheritedItemDetailsInstance);
            this.Discriminator = Discriminator;
        }

        public IVirtualInheritancesProvider VirtualInheritancesProvider { get; }
        public string InheritedFromName { get; }
        public string Discriminator { get; }
    }

    public interface IVirtualInheritancesProvider
    {
        /// <summary>
        /// Provides Virtual Inheritances map
        /// </summary>
        /// <returns>DiscriminatorValue (usually Kind Property value) to Type map </returns>
        Dictionary<string, VirtuallyInheritedObjectProperties> GetVirtualInheritances(string documentVersion);
    }
    public class VirtuallyInheritedObjectProperties
    {
        public VirtuallyInheritedObjectProperties(Type inheritesClassType)
        {
            InheritesClassType = inheritesClassType;
        }

        public VirtuallyInheritedObjectProperties(string inheritesClassName, string childClassName, string inheritesClassDescription, Type innerPropertyClassType, string innerPropertyName = "properties")
        {
            InnerPropertyClassType = innerPropertyClassType;
            InnerPropertyName = innerPropertyName;
            InheritesClassName = inheritesClassName;
            InnerPropertyClassName = childClassName;
            InheritesClassDescription = inheritesClassDescription;
            InnerPropertyName = innerPropertyName;
            if (InheritesClassName == InnerPropertyClassName)
            {
                throw new Exception($"Circular reference detected when generating swagger schema {InheritesClassName}");
            }
        }
        public Type InheritesClassType { get; }
        public string InheritesClassName { get; }
        public string InnerPropertyClassName { get; }
        public string InheritesClassDescription { get; }
        public Type InnerPropertyClassType { get; }
        public string InnerPropertyName { get; }
    }

    [ClientFlatten]
    [SwaggerSchemaNameStrategyAttribute(NamingStrategy.Custom, typeof(GenericArgumentPropertiesSuffixRemover))]
    public class VirtualInheritecePropertiesWrapperTemplate<TPropsType>
    {
        public TPropsType Properties { get; set; }
    }

    public class GenericArgumentPropertiesSuffixRemover : ICustomSchemaNameProvider
    {
        public string GetCustomName(Type type)
        {
            var genericArg = type.GetGenericArguments().Single();
            return genericArg.Name.Replace("Properties", string.Empty);
        }
    }
}
