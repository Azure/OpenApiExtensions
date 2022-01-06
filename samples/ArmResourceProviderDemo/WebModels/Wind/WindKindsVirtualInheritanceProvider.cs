using System.Collections.Generic;
using Microsoft.Azure.OpenApiExtensions.Attributes;

namespace ArmResourceProviderDemo.WebModels.Wind
{
    public class WindKindsVirtualInheritanceProvider : IVirtualInheritancesProvider
    {
        public Dictionary<string, VirtuallyInheritedObjectProperties> GetVirtualInheritances(string documentVersion)
        {
            var kindToInheritedMap = new Dictionary<string, VirtuallyInheritedObjectProperties>();

            kindToInheritedMap["IsraelWindKind"] = new VirtuallyInheritedObjectProperties(
                inheritesClassName: "WindIsrael",
                childClassName: nameof(WindIsraelProperties),
                innerPropertyClassType: typeof(WindIsraelProperties),
                inheritesClassDescription: "Wind Israel description");
            kindToInheritedMap["IndiaWindKind"] = new VirtuallyInheritedObjectProperties(
                inheritesClassName: "WindIndia",
                childClassName: nameof(WindIndiaProperties),
                innerPropertyClassType: typeof(WindIndiaProperties),
                inheritesClassDescription: "Wind India description");

            return kindToInheritedMap;
        }
    }
}
