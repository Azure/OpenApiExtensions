namespace SimpleKindArmResourceProviderDemo.WebModels.Wind
{
    public class WindBaseProperties
    {
        public int BaseProperty { get; set; }
    }


    public class WindIsraelProperties : WindBaseProperties
    {
        public int IsraelProperty { get; set; }
    }

    public class WindIndiaProperties : WindBaseProperties
    {
        public int IndiaProperty { get; set; }
    }
}
