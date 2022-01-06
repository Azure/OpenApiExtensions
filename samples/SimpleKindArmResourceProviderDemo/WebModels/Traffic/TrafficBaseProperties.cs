﻿namespace SimpleKindArmResourceProviderDemo.WebModels.Traffic
{
    public class TrafficBaseProperties
    {
        public int BaseProperty { get; set; }
    }

    public class TrafficEnglandProperties : TrafficBaseProperties
    {
        public int EnglandProperty { get; set; }
    }

    public class TrafficUsaProperties : TrafficBaseProperties
    {
        public int UsaProperty { get; set; }
    }
}
