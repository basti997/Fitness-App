using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace WorkoutTracker.Tests.Integration
{
    // Uses the API assembly marker to boot your API in-memory
    public class ApiValidationTests : IClassFixture<WebApplicationFactory<WorkoutTracker.API.ApiAssemblyMarker>>
    {
        private readonly WebApplicationFactory<WorkoutTracker.API.ApiAssemblyMarker> _factory;

        public ApiValidationTests(WebApplicationFactory<WorkoutTracker.API.ApiAssemblyMarker> factory)
        {
            _factory = factory; // defaults to Development
        }

        [Fact]
        public async Task Workout_GetById_InvalidId_ReturnsBadRequest()
        {
            // GET /api/workout/0 -> controller checks id <= 0 and returns 400 without hitting DB
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/workout/0");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Workout_GetByUser_InvalidUserId_ReturnsBadRequest()
        {
            // GET /api/workout/user/0 -> controller checks userId <= 0 and returns 400
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/workout/user/0");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Workout_Create_MissingOrInvalidBody_ReturnsBadRequest()
        {
            var client = _factory.CreateClient();

            // Case 1: Missing body (empty content) -> model binding yields null dto; controller returns 400
            var empty = new StringContent("", Encoding.UTF8, "application/json");
            var respEmpty = await client.PostAsync("/api/workout", empty);
            Assert.Equal(HttpStatusCode.BadRequest, respEmpty.StatusCode);

            // Case 2: Invalid body: UserId <= 0 -> controller returns 400 before any DB call
            var invalid = JsonContent.Create(new { userId = 0 });
            var respInvalid = await client.PostAsync("/api/workout", invalid);
            Assert.Equal(HttpStatusCode.BadRequest, respInvalid.StatusCode);
        }

        [Fact]
        public async Task User_Login_MissingEmail_ReturnsBadRequest()
        {
            // POST /api/user/login with empty email -> controller returns 400
            var client = _factory.CreateClient();
            var body = JsonContent.Create(new { email = "", password = "any" });
            var response = await client.PostAsync("/api/user/login", body);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Exercise_Post_MissingBody_ReturnsBadRequest()
        {
            // POST /api/exercise with no JSON body -> controller sees null and returns 400
            var client = _factory.CreateClient();
            var emptyJson = new StringContent("", Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/exercise", emptyJson);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Exercise_Put_MissingBody_ReturnsBadRequest()
        {
            // PUT /api/exercise with no JSON body -> controller sees null and returns 400
            var client = _factory.CreateClient();
            var emptyJson = new StringContent("", Encoding.UTF8, "application/json");

            var response = await client.PutAsync("/api/exercise", emptyJson);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task WorkoutSet_Put_MissingBody_ReturnsBadRequest()
        {
            // PUT /api/workoutset with no JSON body -> controller sees null and returns 400
            var client = _factory.CreateClient();
            var emptyJson = new StringContent("", Encoding.UTF8, "application/json");

            var response = await client.PutAsync("/api/workoutset", emptyJson);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}