using System;
using System.Linq;

namespace Microsoft.Azure.OpenApiExtensions.Helpers
{
    public class ArmResourceWrapperNameProvider : ICustomSchemaNameProvider
    {
        /// <summary>
        /// Get Custom Name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetCustomName(Type type)
        {
            var genericArg = type.GetGenericArguments().Single();
            return genericArg.Name.Replace("Properties", "Model");
        }
    }
}
