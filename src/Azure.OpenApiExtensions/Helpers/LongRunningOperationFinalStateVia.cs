// <copyright file="LongRunningOperationFinalStateVia.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Helpers
{
    /// <summary>
    /// Long running operation types for swagger extension filter attribute.
    /// </summary>
#pragma warning disable CA1717 // Only FlagsAttribute enums should have plural names
    public enum LongRunningOperationFinalStateVia
#pragma warning restore CA1717 // Only FlagsAttribute enums should have plural names
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
}
