using System;

namespace Microsoft.Azure.OpenApiExtensions.Attributes
{
    /// <summary>
    /// Describes the format for specifying examples for request and response of an operation in an OpenAPI definition.
    /// In openApi documents, operations will have this field by default, with example file at #./examples/{operationId}.json.
    /// </summary>
    /// <see href="https://github.com/Azure/autorest/tree/master/docs/extensions#x-ms-examples">x-ms-examples.</see>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ExampleAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="title">OperationId Eg: DerivedModels_Get.</param>
        public ExampleAttribute(string folder, string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            Title = title;
            FilePath = $"{folder}/" + title;
        }

        /// <summary>
        /// Gets operationId Eg: DerivedModels_Get.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets file path of examples file Eg: #./examples/DerivedModels_Get.json.
        /// </summary>
        public string FilePath { get; }
    }
}
