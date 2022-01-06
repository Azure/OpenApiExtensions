using System;

namespace Microsoft.Azure.OpenApiExtensions.Attributes
{
    public enum CommonObjectType
    {
        ResourceProviderCommonDefinition,
        GlobalCommonDefinition
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CustomSwaggerSchemaInheritanceAttribute : Attribute
    {
        public CustomSwaggerSchemaInheritanceAttribute(string externalSchemaName, string[] notInheritedPropertiesName, CommonObjectType definitionLevel = CommonObjectType.ResourceProviderCommonDefinition)
        {
            ExternalSchemaName = externalSchemaName;
            DefinitionLevel = definitionLevel;
            NotInheritedPropertiesName = notInheritedPropertiesName;
        }

        public string ExternalSchemaName { get; private set; }
        public CommonObjectType DefinitionLevel { get; private set; }
        public string[] NotInheritedPropertiesName { get; }
        public string SchemaDescriptionFormat { get; private set; }
    }
}
