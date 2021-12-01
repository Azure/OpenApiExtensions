using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace ArmResourceProviderDemo.WebModels
{    
    public class TrafficBaseProperties
    {
        public int BaseProperty { get; set; }
    }


    public class TrafficIsraelProperties : TrafficBaseProperties
    {
        public int IsraelProperty { get; set; }

    }

    public class TrafficIndiaProperties : TrafficBaseProperties
    {
        public int IndiaProperty { get; set; }
    }   
}
