using Microsoft.Azure.OpenApiExtensions.Attributes;

namespace ArmResourceProviderDemo.WebModels.Speed
{
    [SwaggerSchemaNameStrategy(NamingStrategy.ApplyToParentWrapper, "SpeedWrapper")]
    [SwaggerSchemaNameStrategy("SpeedCustomName")]

    public class SpeedProperties
    {
        public int Speed { get; set; }
    }
}
