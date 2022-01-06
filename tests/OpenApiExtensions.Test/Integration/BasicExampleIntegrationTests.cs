using System.Net;
using System.Net.Http;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using ApiFixture = OpenApiExtensions.Test.Integration.CustomWebApplicationFactory<BasicWebAppDemo.Program>;

namespace OpenApiExtensions.Test.Integration
{
    [Collection("IntegrationTests")]
    public class BasicExampleIntegrationTests : IClassFixture<ApiFixture>
    {
        private readonly HttpClient _client;
        private readonly ApiFixture _factory;

        public BasicExampleIntegrationTests(ApiFixture factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData("/swagger")]
        [InlineData("/swagger/2021-09-01-preview/swagger.json")]
        [InlineData("/swagger/2022-01-01-preview/swagger.json")]
        [InlineData("/swagger/2021-10-01/swagger.json")]
        public async void Get_SwaggerPerVersion_Ok(string url)
        {
            // Act
            var response = await _client.GetAsync(url);
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("/swagger/2021-09-01-preview/swagger.json")]
        [InlineData("/swagger/2022-01-01-preview/swagger.json")]
        [InlineData("/swagger/2021-10-01/swagger.json")]
        public async void Get_SwaggerPerVersion_PathsNotEmpty(string url)
        {
            // Act
            var response = await _client.GetAsync(url);
            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<SwaggerDocument>(content);
            Assert.NotEmpty(obj.Paths.Keys);
            //Assert.Equal(apiVersion, obj.Info.Version);
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("/swagger/2021-09-01-preview/swagger.json")]
        [InlineData("/swagger/2021-10-01/swagger.json")]
        public async void Get_SwaggerPerVersion_PolymorphsimClassesReturned(string url)
        {
            // Act
            var response = await _client.GetAsync(url);
            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<SwaggerDocument>(content, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.All });
            obj.Definitions.Should().ContainKey("WeatherForecastNetanya");
        }

        [Fact]
        public async void Get_ODataApi_OdataParameterAreSpecified()
        {
            // Act
            var response = await _client.GetAsync("/swagger/2021-09-01-preview/swagger.json");
            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<SwaggerDocument>(content, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.All });

            var odataEndpointParams = obj.Paths["/WeatherForecast/test1/{val}"]["get"].Parameters;
            odataEndpointParams.Should().Contain(o => o.ContainsKey("$ref") && o["$ref"].ToString().Contains("ODataFilter"));
            odataEndpointParams.Should().Contain(o => o.ContainsKey("$ref") && o["$ref"].ToString().Contains("ODataTop"));
        }     
    }
}
