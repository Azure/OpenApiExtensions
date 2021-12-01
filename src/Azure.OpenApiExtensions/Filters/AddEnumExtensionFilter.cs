//-----------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------

namespace Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Filters
{
    using Microsoft.OpenApi.Any;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using System;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Adds x-ms-enum to a property enum type. Adds extension attributes to indicate AutoRest to model enum as string
    /// </summary>
    /// <see href="https://github.com/Azure/autorest/tree/master/docs/extensions#x-ms-enum">x-ms-enum</see>
    public class AddEnumExtensionFilter : ISchemaFilter
    {
        private bool modelAsString = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddEnumExtensionFilter"/> class.
        /// </summary>
        public AddEnumExtensionFilter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddEnumExtensionFilter"/> class.
        /// </summary>
        /// <param name="modelAsString">Extend AddEnumExtensionFilter to support model as string false along with true.</param>
        public AddEnumExtensionFilter(bool modelAsString)
        {
            this.modelAsString = modelAsString;
        }

        /// <summary>
        /// Applies filter.
        /// </summary>
        /// <param name="schema">OpenApiSchema.</param>
        /// <param name="context">DocumentFilterContext.</param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;
            if (type.IsEnum)
            {
                schema.Type = modelAsString ? "string" : schema.Type;
                if (schema.AllOf?.Count == 0)
                {
                    var xMsEnumExtensionObject = new OpenApiObject
                    {
                        ["name"] = new OpenApiString(type.Name),
                        ["modelAsString"] = new OpenApiBoolean(this.modelAsString),
                    };

                    schema.Format = null;
                    if (modelAsString)
                    {
                        var values = new OpenApiArray();
                        schema.Enum.Clear();
                        foreach (string enumName in Enum.GetNames(context.Type))
                        {
                            System.Reflection.MemberInfo memberInfo = context.Type.GetMember(enumName).FirstOrDefault(m => m.DeclaringType == context.Type);
                            EnumMemberAttribute enumMemberAttribute = memberInfo == null
                             ? null
                             : memberInfo.GetCustomAttributes(typeof(EnumMemberAttribute), false).OfType<EnumMemberAttribute>().FirstOrDefault();

                            string label = enumMemberAttribute == null || string.IsNullOrWhiteSpace(enumMemberAttribute.Value)
                             ? enumName
                             : enumMemberAttribute.Value;

                            schema.Enum.Add(new OpenApiString(label));
                            var val = new OpenApiObject();
                            val.Add("value", new OpenApiString(label));
                            values.Add(val);
                        }                
                        xMsEnumExtensionObject.Add("values", values);
                        schema.Example = schema.Enum.First();
                    }
                    schema.Extensions.Add("x-ms-enum", xMsEnumExtensionObject);
                }
            }
        }
    }
}
