using System;
using System.Linq;

namespace Microsoft.Azure.OpenApiExtensions.Helpers
{
    public class ArmResourceWrapperNameProvider : ICustomSchemaNameProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelType">Type Must be generic and have a single generic Arg</param>
        /// <returns></returns>
        public string GetCustomName(Type type)
        {
            var genericArg = type.GetGenericArguments().Single();
            return genericArg.Name.Replace("Properties", "Model");
        }
    }
}
