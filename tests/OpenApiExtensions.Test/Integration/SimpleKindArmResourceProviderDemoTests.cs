using System.Net;
using System.Net.Http;
using Xunit;
using ApiFixture = OpenApiExtensions.Test.Integration.CustomWebApplicationFactory<SimpleKindArmResourceProviderDemo.Program>;

namespace OpenApiExtensions.Test.Integration
{
    [Collection("IntegrationTests")]
    public class SimpleKindArmResourceProviderDemoTests : IClassFixture<ApiFixture>
    {
        private readonly HttpClient _client;        

        public SimpleKindArmResourceProviderDemoTests(ApiFixture factory)
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
    }
}
