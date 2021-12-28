using System;

namespace Microsoft.Azure.OpenApiExtensions.Attributes
{
    public enum LongRunningOperationFinalStateVia
    {
        /// <summary>
        /// The final response will be available at the uri pointed to by the header Azure-AsyncOperation.
        /// </summary>
        AzureAsyncOperation,

        /// <summary>
        /// The final response will be available at the uri pointed to by the header Location
        /// </summary>
        Location,

        /// <summary>
        /// The final response will be available via GET at the original resource URI
        /// </summary>
        OriginalUri,
    }

    /// <summary>
    /// Some REST operations can take a long time to complete. Although REST is not supposed to be stateful,
    /// some operations are made asynchronous while waiting for the state machine to create the resources,
    /// and will reply before the operation on resources are completed.
    /// When x-ms-long-running-operation is specified, there should also be a x-ms-long-running-operation-options specified.
    /// This attribute should be used when the final state is conveyed using the location header.
    /// </summary>
    /// <see href="https://github.com/Azure/azure-resource-manager-rpc/blob/master/v1.0/Addendum.md#asynchronous-operations">Asynchronous Operations.</see>
    /// <see href="https://github.com/Azure/autorest/tree/master/docs/extensions#x-ms-long-running-operation">x-ms-long-running-operation.</see>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class LongRunningOperationAttribute : Attribute
    {
        /// <summary>
        /// Final state via enum.
        /// </summary>
        public LongRunningOperationFinalStateVia FinalStateVia { get; set; }
    }
}
