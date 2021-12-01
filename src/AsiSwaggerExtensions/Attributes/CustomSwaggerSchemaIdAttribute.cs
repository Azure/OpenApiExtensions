//-----------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------

namespace Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Attributes
{
    using System;
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CustomSwaggerSchemaIdAttribute : Attribute
    {        
        public CustomSwaggerSchemaIdAttribute(string schemaPrefix)
        {
            SchemaPrefix = schemaPrefix;
        }
        
        public CustomSwaggerSchemaIdAttribute(string schemaPrefix, string showOnlyOnDocumentVersion)
        {
            SchemaPrefix = schemaPrefix;
            ShowOnlyOnDocumentVersion = showOnlyOnDocumentVersion;
        }

        public string SchemaPrefix { get; private set; }

 
        public string ShowOnlyOnDocumentVersion { get; private set; }
    }
}
