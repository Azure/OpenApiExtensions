using Microsoft.Azure.OpenApiExtensions.Helpers;
using System;

namespace Microsoft.Azure.OpenApiExtensions.Attributes
{
    public enum NamingStrategy
    {
        ConcreteNamingConcatanation,
        Custom,
        ApplyToParentWrapper,
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum, AllowMultiple = true, Inherited = true)]
    public class SwaggerSchemaNameStrategyAttribute : Attribute
    {
        public SwaggerSchemaNameStrategyAttribute(NamingStrategy namingStrategy, Type customNameProvider = null)
        {
            if (namingStrategy == NamingStrategy.Custom && (customNameProvider is null || !typeof(ICustomSchemaNameProvider).IsAssignableFrom(customNameProvider)))
            {
                throw new ArgumentNullException(nameof(customNameProvider));
            }

            NamingStrategy = namingStrategy;
            CustomNameProvider = (ICustomSchemaNameProvider)Activator.CreateInstance(customNameProvider);
        }

        public SwaggerSchemaNameStrategyAttribute(string Name)
        {
            NamingStrategy = NamingStrategy.Custom;
            CustomNameProvider = new ConstNameProvider(Name);
        }

        public SwaggerSchemaNameStrategyAttribute(NamingStrategy namingStrategy, string Name)
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
