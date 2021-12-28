using Microsoft.Azure.OpenApiExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.OpenApiExtensions.Helpers
{
    public interface IVirtuallyInheritedItemDetails<T> where T : Enum
    {        
        VirtuallyInheritedObjectProperties Provide(T kindName);
    }

    public abstract class VirtuallyInheritedItemDetailsAbstract<T> : IVirtuallyInheritedItemDetails<T> where T : Enum
    {
        public VirtuallyInheritedObjectProperties Provide(T kindValue)
        {
            return new VirtuallyInheritedObjectProperties(
                        inheritesClassName: GetParentSwaggerDisplayName(kindValue),
                        childClassName: GetChildSwaggerDisplayName(kindValue),
                        inheritesClassDescription: GetDescription(kindValue),
                        innerPropertyClassType: GetPropertiesClass(kindValue));
        }

        protected abstract Type GetPropertiesClass(T kindValue);
        protected abstract string GetDescription(T kindValue);
        protected abstract string GetChildSwaggerDisplayName(T kindValue);
        protected abstract string GetParentSwaggerDisplayName(T kindValue);
    }
}
