// <copyright file="HideInDocsAttribute.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Attributes
{
    using System;

    /// <summary>
    /// A marker attribute which can be applied to an API or controller to hide from docs.
    /// <see cref="HideInDocsFilter"/> Swagger document filter is going to use it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class HideInDocsAttribute : Attribute
    {
    }
}
