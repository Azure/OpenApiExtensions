using System;
using Microsoft.Azure.OpenApiExtensions.Helpers;

namespace SimpleKindArmResourceProviderDemo.WebModels.Traffic
{
    public class TrafficDetailedItemProvider : VirtuallyInheritedItemDetailsAbstract<CountryKind>
    {
        protected override string GetChildSwaggerDisplayName(CountryKind kindValue)
        {

            return $"{kindValue.ToString().ToUpper()}TrafficProperties";
        }

        protected override string GetDescription(CountryKind kindValue)
        {
            return $"kind:{kindValue} description";

        }

        protected override string GetParentSwaggerDisplayName(CountryKind kindValue)
        {
            return $"{kindValue.ToString().ToUpper()}Traffic";
        }

        protected override Type GetPropertiesClass(CountryKind kindValue)
        {
            string propertiesClassName = $"SimpleKindArmResourceProviderDemo.WebModels.Traffic.Traffic{ToCamelCase(kindValue)}Properties";
            return GetType().Assembly.GetType(propertiesClassName);
        }

        private string ToCamelCase(CountryKind kindValue)
        {
            return string.Concat(kindValue.ToString()[0].ToString().ToUpper(), kindValue.ToString().ToLower().AsSpan(1));
        }
    }
}
