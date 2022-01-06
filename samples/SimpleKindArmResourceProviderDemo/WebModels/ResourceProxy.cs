namespace SimpleKindArmResourceProviderDemo.WebModels
{
    public class ResourceProxy<TProps> : Resource
    {
        public string Kind { get; set; }

        public TProps Properties { get; set; }
    }
}
