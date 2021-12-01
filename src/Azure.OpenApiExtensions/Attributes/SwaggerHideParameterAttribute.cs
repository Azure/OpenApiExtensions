//-----------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------

namespace Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Attributes
{
    using System;

    /// <summary>
    /// Describes the format for specifying examples for request and response of an operation in an OpenAPI definition.
    /// In openApi documents, operations will have this field by default, with example file at #./examples/{operationId}.json.
    /// </summary>    
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class SwaggerHideParameterAttribute : Attribute
    {
    }
}
