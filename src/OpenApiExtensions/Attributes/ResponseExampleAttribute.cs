using Microsoft.Azure.OpenApiExtensions.Helpers;
using System;

namespace Microsoft.Azure.OpenApiExtensions.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ResponseExampleAttribute : Attribute
    {
        /// <summary>
        /// Init new class that implements ExampleTypeProvider, to provide example for a controller's response
        /// </summary>
        /// <param name="httpCode"></param>
        /// <param name="exampleTypeProvider">The type of ExampleTypeProvider to initiate</param>
        /// <param name="exampleName">Optional. For support multiple examples by example name in single ExampleTypeProvider</param>

        public ResponseExampleAttribute(int httpCode, Type exampleTypeProvider, string exampleName = null)
        {
            HttpCode = httpCode;
            ExampleTypeProvider = exampleTypeProvider ?? throw new ArgumentNullException(nameof(exampleTypeProvider));

            if (!typeof(IExamplesProvider).IsAssignableFrom(exampleTypeProvider))
            {
                throw new InvalidOperationException($"Example object {exampleTypeProvider.Name} must implement the interface {nameof(IExamplesProvider)}");
            }

            if (exampleName == null)
            {
                ExampleProviderInstance = (IExamplesProvider)Activator.CreateInstance(exampleTypeProvider);
            } 
            else
            {
                ExampleProviderInstance = (IExamplesProvider)Activator.CreateInstance(exampleTypeProvider, exampleName);
            }
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
        /// <summary>
        /// Init new class that implements ExampleTypeProvider, to provide example for a controller's request
        /// </summary>
        /// <param name="exampleTypeProvider">The type of ExampleTypeProvider to initiate</param>
        /// <param name="exampleName">Optional. For support multiple examples by example name in single ExampleTypeProvider</param>
        public RequestExampleAttribute(Type exampleTypeProvider, string exampleName = null)
        {
            ExampleTypeProvider = exampleTypeProvider ?? throw new ArgumentNullException(nameof(exampleTypeProvider));

            if (!typeof(IExamplesProvider).IsAssignableFrom(exampleTypeProvider))
            {
                throw new InvalidOperationException($"Example object {exampleTypeProvider.Name} must implement the interface {nameof(IExamplesProvider)}");
            }

            if (exampleName == null)
            {
                ExampleProviderInstance = (IExamplesProvider)Activator.CreateInstance(exampleTypeProvider);
            }
            else
            {
                ExampleProviderInstance = (IExamplesProvider)Activator.CreateInstance(exampleTypeProvider, exampleName);
            }
            
        }

        public Type ExampleTypeProvider { get; }
        public IExamplesProvider ExampleProviderInstance { get; private set; }
    }
}
