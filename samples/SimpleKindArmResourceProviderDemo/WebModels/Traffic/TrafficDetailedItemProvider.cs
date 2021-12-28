using Microsoft.Azure.OpenApiExtensions.Helpers;
using SimpleKindArmResourceProviderDemo.WebModels.Wind;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleKindArmResourceProviderDemo.WebModels.Traffic
{
    public class TrafficDetailedItemProvider : VirtuallyInheritedItemDetailsAbstract<CountryKind>
    {
        protected override string GetChildSwaggerDisplayName(CountryKind kindValue)
        {
            
            return $"{kindValue.ToString().ToUpper()}WindProperties";
        }

        protected override string GetDescription(CountryKind kindValue)
        {
              return $"kind:{kindValue} description";
                          
        }

        protected override string GetParentSwaggerDisplayName(CountryKind kindValue)
        {
            return $"{kindValue.ToString().ToUpper()}Wind";
        }

        protected override Type GetPropertiesClass(CountryKind kindValue)
        {
            TextInfo info = CultureInfo.CurrentCulture.TextInfo;
            return this.GetType().Assembly.GetType($"SimpleKindArmResourceProviderDemo.WebModels.Wind.Wind{info.ToTitleCase(kindValue.ToString())}Properties");

        }
    }
}
