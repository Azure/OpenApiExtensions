using Microsoft.Azure.OpenApiExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.OpenApiExtensions.Helpers
{
    public class SimpleKindVirtualInheritanceProvider<T> : IVirtualInheritancesProvider where T : Enum
    {
        private IVirtuallyInheritedItemDetails<T> _virtuallyInheritedItemDetails;
        private Type _kindEnum;

        public SimpleKindVirtualInheritanceProvider(Type kindEnum, IVirtuallyInheritedItemDetails<T> virtuallyInheritedItemDetails)
        {
            _kindEnum = kindEnum;
            _virtuallyInheritedItemDetails = virtuallyInheritedItemDetails;
        }

        public Dictionary<string, VirtuallyInheritedObjectProperties> GetVirtualInheritances(string documentVersion)
        {
            var kindToInheritedMap = new Dictionary<string, VirtuallyInheritedObjectProperties>();
            foreach (T connectorKind in Enum.GetValues(_kindEnum))
            {
                kindToInheritedMap[connectorKind.ToString()] = _virtuallyInheritedItemDetails.Provide(connectorKind);
            }

            return kindToInheritedMap;
        }
    }
}
