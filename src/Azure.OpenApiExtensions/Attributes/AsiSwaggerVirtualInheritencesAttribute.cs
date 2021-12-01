//-----------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------

namespace Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Attributes
{
    using AsiSwaggerExtensions.Helpers;
    using Microsoft.Azure.Global.Services.Common.Service.OpenApi.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AsiSwaggerVirtualInheritancesAttribute : Attribute
    {
        public AsiSwaggerVirtualInheritancesAttribute(Type VirtualInheritancesProviderType, string InheritedFromName, string Discriminator = "kind")
        {
            if ((VirtualInheritancesProviderType is null || !typeof(IVirtualInheritancesProvider).IsAssignableFrom(VirtualInheritancesProviderType)))
            {
                throw new ArgumentNullException(nameof(VirtualInheritancesProviderType));
            }

            this.VirtualInheritancesProvider = (IVirtualInheritancesProvider)Activator.CreateInstance(VirtualInheritancesProviderType);
            this.InheritedFromName = InheritedFromName;
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
        public Type InheritedSubClassType { get; set; }
    }

    [ClientFlatten]
    [CustomSwaggerGenericsSchemaNameStrategy(NamingStrategy.Custom, typeof(GenericArgumentPropertiesSuffixRemover))]
    public class VirtualInheritecePropertiesWrapperTemplate<TPropsType>
    {
        public TPropsType properties { get; set; }
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
