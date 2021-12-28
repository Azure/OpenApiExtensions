using Microsoft.Azure.OpenApiExtensions.Helpers;
using System;

namespace Microsoft.Azure.OpenApiExtensions.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ResponseExampleAttribute : Attribute
    {

        public ResponseExampleAttribute(int httpCode, Type exampleTypeProvider)
        {
            HttpCode = httpCode;
            ExampleTypeProvider = exampleTypeProvider ?? throw new ArgumentNullException(nameof(exampleTypeProvider));

            if (!typeof(IExamplesProvider).IsAssignableFrom(exampleTypeProvider))
            {
                throw new InvalidOperationException($"Example object {exampleTypeProvider.Name} must implement the interface {nameof(IExamplesProvider)}");
            }
            ExampleProviderInstance = (IExamplesProvider)Activator.CreateInstance(exampleTypeProvider);
        }

        /// <summary>
        /// Gets operationId Eg: DerivedModels_Get.
        /// </summary>
        public int HttpCode { get; }

        /// <summary>
        /// Gets file path of examples file Eg: #./examples/DerivedModels_Get.json.
        /// </summary>
        public Type ExampleTypeProvider { get; }
        public IExamplesProvider ExampleProviderInstance { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequestExampleAttribute : Attribute
    {

        public RequestExampleAttribute(Type exampleTypeProvider)
        {
            ExampleTypeProvider = exampleTypeProvider ?? throw new ArgumentNullException(nameof(exampleTypeProvider));

            if (!typeof(IExamplesProvider).IsAssignableFrom(exampleTypeProvider))
            {
                throw new InvalidOperationException($"Example object {exampleTypeProvider.Name} must implement the interface {nameof(IExamplesProvider)}");
            }
            ExampleProviderInstance = (IExamplesProvider)Activator.CreateInstance(exampleTypeProvider);
        }

        public Type ExampleTypeProvider { get; }
        public IExamplesProvider ExampleProviderInstance { get; private set; }
    }
}
