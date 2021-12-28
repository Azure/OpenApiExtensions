using Microsoft.Azure.OpenApiExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace ArmResourceProviderDemo.WebModels.Traffic
{
    public class TrafficKindsVirtualInheritanceProvider : IVirtualInheritancesProvider
    {
        public Dictionary<string, VirtuallyInheritedObjectProperties> GetVirtualInheritances(string documentVersion)
        {
            var kindToInheritedMap = new Dictionary<string, VirtuallyInheritedObjectProperties>();
            Type concreteTemplateType = typeof(VirtualInheritecePropertiesWrapperTemplate<>);
            Type israelTrafficConcreteType = concreteTemplateType.MakeGenericType(typeof(TrafficIsraelProperties));
            Type indiaTrafficConcreteType = concreteTemplateType.MakeGenericType(typeof(TrafficIndiaProperties));
            kindToInheritedMap["Israel"] = new VirtuallyInheritedObjectProperties(israelTrafficConcreteType);
            kindToInheritedMap["India"] = new VirtuallyInheritedObjectProperties(indiaTrafficConcreteType);

            return kindToInheritedMap;
        }
    }
}
