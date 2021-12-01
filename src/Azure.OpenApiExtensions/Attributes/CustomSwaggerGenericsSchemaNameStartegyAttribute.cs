//-----------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------

namespace Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Attributes
{
    using AsiSwaggerExtensions.Helpers;
    using System;

    public enum NamingStrategy
    {
        ConcreteNamingConcatanation,
        Custom,
        ApplyToParentWrapper,
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CustomSwaggerGenericsSchemaNameStrategy : Attribute
    {
        public CustomSwaggerGenericsSchemaNameStrategy(NamingStrategy namingStrategy, Type customNameProvider = null)
        {
            if (namingStrategy == NamingStrategy.Custom && (customNameProvider is null || !typeof(ICustomSchemaNameProvider).IsAssignableFrom(customNameProvider)))
            {
                throw new ArgumentNullException(nameof(customNameProvider));
            }

            NamingStrategy = namingStrategy;
            CustomNameProvider = (ICustomSchemaNameProvider)Activator.CreateInstance(customNameProvider);
        }

        public CustomSwaggerGenericsSchemaNameStrategy(string Name)
        {
            NamingStrategy = NamingStrategy.Custom;
            CustomNameProvider = new ConstNameProvider(Name);
        }

        public CustomSwaggerGenericsSchemaNameStrategy(NamingStrategy namingStrategy, string Name)
        {
            NamingStrategy = namingStrategy;
            CustomNameProvider = new ConstNameProvider(Name);
        }

        public NamingStrategy NamingStrategy { get; }
        public ICustomSchemaNameProvider CustomNameProvider { get; private set; }
    }

    public class ConstNameProvider : ICustomSchemaNameProvider
    {
        public string _name;
        public ConstNameProvider(string name)
        {
            _name = name;
        }
        public string GetCustomName(Type type) => _name;
    }
}
