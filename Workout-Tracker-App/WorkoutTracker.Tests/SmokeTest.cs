using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace WorkoutTracker.Tests.Integration
{
    // Use the fully qualified type so there’s no ambiguity
    public class ApiSmokeTests : IClassFixture<WebApplicationFactory<WorkoutTracker.API.ApiAssemblyMarker>>
    {
        private readonly WebApplicationFactory<WorkoutTracker.API.ApiAssemblyMarker> _factory;

        public ApiSmokeTests(WebApplicationFactory<WorkoutTracker.API.ApiAssemblyMarker> factory)
        {
            _factory = factory; // defaults to Development
        }

        [Fact]
        public async Task Root_ReturnsNotFound_ButNoServerError()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync("/");
            Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task SwaggerJson_ReturnsSuccess_InDevelopment()
        {
            var client = _factory.CreateClient();
            var swagger = await client.GetAsync("/swagger/v1/swagger.json");
            swagger.EnsureSuccessStatusCode();
        }
    }
}