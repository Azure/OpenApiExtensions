using Microsoft.Azure.OpenApiExtensions.Helpers;
using SimpleKindArmResourceProviderDemo.WebModels.Traffic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleKindArmResourceProviderDemo.WebModels.Wind
{
    public class WindDetailedItemProvider : VirtuallyInheritedItemDetailsAbstract<CountryKind>
    {
        protected override string GetChildSwaggerDisplayName(CountryKind kindValue)
        {
            return $"{kindValue}Properties";
        }

        protected override string GetDescription(CountryKind kindValue)
        {
            switch (kindValue)
            {
                case CountryKind.USA:
                {
                        return "Kind for United States of America";
                }
                case CountryKind.ENGLAND:
                {
                    return "Kind for Great Britan";
                }
                default:
                {
                    return $"Other kind:{kindValue}";
                }
            }
        }

        protected override string GetParentSwaggerDisplayName(CountryKind kindValue)
        {
            return $"{kindValue}CountryKind";
        }

        protected override Type GetPropertiesClass(CountryKind kindValue)
        {
            switch (kindValue)
            {
                case CountryKind.USA:
                {
                        return typeof(TrafficUsaProperties);
                    }
                case CountryKind.ENGLAND:
                {
                        return typeof(TrafficEnglandProperties);
                }
                default:
                {
                    return typeof(TrafficBaseProperties);
                }
            }
        }
    }
}
