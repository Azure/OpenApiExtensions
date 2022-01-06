using System;

namespace Microsoft.Azure.OpenApiExtensions.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CustomSwaggerSchemaAttribute : Attribute
    {
        /// <param name="descriptionFormat">DescriptionFormat Eg: List of {GenericType} models</param>
        public CustomSwaggerSchemaAttribute(string descriptionFormat)
        {
            DescriptionFormat = descriptionFormat;
        }

        /// <summary>
        /// Can be string of format with the following Directives: {GenericType}
        /// </summary>
        /// <example>List of {GenericType} models</example>
        public string DescriptionFormat { get; }
    }
}
