using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Xunit;
using FluentAssertions;
using ApiFixture = OpenApiExtensions.Test.Integration.CustomWebApplicationFactory<ArmResourceProviderDemo.Program>;

namespace OpenApiExtensions.Test.Integration
{
    [Collection("IntegrationTests")]
    public class ArmResourceProviderDemoTests : IClassFixture<ApiFixture>
    {
        private readonly HttpClient _client;        

        public ArmResourceProviderDemoTests(ApiFixture factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async void Get_SwaggerArmDocument_Ok()
        {
            // Act            
            var response = await _client.GetAsync("/swagger/v1/swagger.json");
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void Get_InheritedModel_Exists()
        {
            // Act            
            var response = await _client.GetAsync("/swagger/v1/swagger.json");
            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<SwaggerDocument>(content, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.All });
            obj.Definitions.Should().ContainKey("WindIsrael");
        }

        [Fact]
        public async void Get_SpeedModelsRenaming_NewNamesReturned()
        {
            // Act            
            var response = await _client.GetAsync("/swagger/v1/swagger.json");
            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<SwaggerDocument>(content, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.All });
            obj.Definitions.Should().ContainKey("SpeedCustomName");
            obj.Definitions.Should().ContainKey("SpeedWrapper");
            obj.Definitions.Should().NotContainKey("SpeedProperties");
        }
    }
}
