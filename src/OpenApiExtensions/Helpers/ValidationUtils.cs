using System;
using System.Reflection;

namespace Microsoft.Azure.OpenApiExtensions.Helpers
{
    class ValidationUtils
    {
        /// <summary>
        /// This method verify if candidateType is baseType<paramref name="genericType"/>
        /// For example: if candiate is of type: "VirtuallyInheritedItemDetails&lt;MyEnum&gt;".
        /// </summary>
        /// <param name="baseType">This is the main type, for example: "List"</param>
        /// <param name="genericType">This is the type of the generics, for example: "string"</param>
        /// <param name="candidateType">The type for validation, for example "List&lt;string&gt;></param>
        public static void ValidateIsAssignableToGenericType(Type baseType, Type genericType, Type candidateType)
        {
            if (candidateType is null)
            {
                throw new ArgumentNullException(nameof(candidateType));
            }

            if (IsAssignableToGenericType(candidateType, baseType) == false)
            {
                throw new ArgumentException($"argument must be assignable from {nameof(baseType)}", candidateType.Name);
            }

            if (IsGenericTypeMatch(genericType, candidateType) == false)
            {
                throw new ArgumentException($"argument must be assignable from {baseType.Name} with generics {genericType}", candidateType.Name);
            }
        }

        private static bool IsGenericTypeMatch(Type genericType, Type candidateType)
        {
            if (candidateType.IsGenericType && candidateType.GetTypeInfo().GenericTypeArguments[0].FullName.Equals(genericType.FullName))
            {
                return true;
            }

            if (candidateType.BaseType == null)
            {
                return false;
            }

            return IsGenericTypeMatch(genericType, candidateType.BaseType);
        }

        private static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            Type baseType = givenType.BaseType;
            if (baseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(baseType, genericType);
        }
    }
}
