using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using StorySpoilerExam.Models;
using System.Net;
using System.Text.Json;


namespace StorySpoilerExam
{
    [TestFixture]
    public class StorySpoilerTests
    {
        private RestClient client;
        private static string createdStoryId;
        
        private const string baseUrl = "https://d3s5nxhwblsjbi.cloudfront.net";

        [OneTimeSetUp]
        public void Setup()
        {
            string token = GetJwtToken("testUser93847", "lkdjgr5820$");

            var options = new RestClientOptions(baseUrl)
            {
                Authenticator = new JwtAuthenticator(token)
            };

            client = new RestClient(options);
        }

        private string GetJwtToken(string username, string password)
        {
            var loginClient = new RestClient(baseUrl);
            var request = new RestRequest("/api/User/Authentication", Method.Post);

            request.AddJsonBody(new { username, password });

            var response = loginClient.Execute(request);

            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);

            return json.GetProperty("accessToken").GetString() ?? string.Empty;

        }

        //Assert examples

        //Example for status code
        // Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        //Example for Message
        // Assert.That(response.Content, Does.Contain("No food revues..."));



        //Change names of all tests

        [Test, Order(1)]
        public void CreateStorySpoilerWithRequiredFields_ShouldReturnCreated()
        {
            var request = new RestRequest("/api/Story/Create", Method.Post);
            var story = new StoryDTO
            {
                title = "New Story Spoiler",
                description = "Here goes a story description",
                url = ""
            };

            request.AddJsonBody(story);
            var response = this.client.Execute(request);

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Expected status code is Created(201).");
            Assert.That(response.Content, Does.Contain("Successfully created!"));

            Assert.That(response.Content, Does.Contain("storyId"), "The returned response should include a storyID.");
            
            createdStoryId = jsonResponse.GetProperty("storyId").GetString()??string.Empty;            
        }

        [Test, Order(2)]

        public void EditStoryTitle_ShouldReturnOk()
        {
            var editRequest = new RestRequest($"/api/Story/Edit/{createdStoryId}", Method.Put);
            var editedStory = new StoryDTO
            {
                title = "Updated Story",
                description = "Updated story description",
                url = ""
            };
            editRequest.AddJsonBody(editedStory);
            var response = this.client.Execute(editRequest);
            var jsonResponse = JsonSerializer.Deserialize<ApiResponseDTO> (response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Should return status code OK (200)");
            Assert.That(jsonResponse.msg, Is.EqualTo("Successfully edited"));
        }

        [Test, Order(3)]

        public void GetAllStories_ShouldReturnList()
        {
            var listRequest = new RestRequest("/api/Story/All", Method.Get);
            var response = this.client.Execute(listRequest);
            var jsonResponse = JsonSerializer.Deserialize<List<ApiResponseDTO>> (response.Content);

            Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
            Assert.That(jsonResponse, Is.Not.Empty);
        }

        [Test, Order(4)]

        public void DeleteStory_ShoudReturnOk()
        {
            var deleteRequest = new RestRequest($"/api/Story/Delete/{createdStoryId}", Method.Delete);
            var response = this.client.Execute(deleteRequest);
            var jsonResponse = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(jsonResponse.msg, Is.EquivalentTo("Deleted successfully!"));
        }

        [Test, Order(5)]

        public void CreateStory_WithoutRequiredFields_ShouldReturnBadRequest()
        {
            var request = new RestRequest("/api/Story/Create", Method.Post);
            var story = new StoryDTO
            {
                title = "",
                description = "",
                url = ""
            };

            request.AddJsonBody(story);
            var response = this.client.Execute(request);

            var jsonResponse = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "Expected status code is Bad Request(400).");
        }


        [Test, Order(6)]

        public void EditNonExistingStory_ShouldReturnNotFound()
        {
            var fakeStoryId = "384kd438";
            var editRequest = new RestRequest($"/api/Story/Edit/{fakeStoryId}", Method.Put);
            var editedStory = new StoryDTO
            {
                title = "Updated Story",
                description = "Updated story description",
                url = ""
            };
            editRequest.AddJsonBody(editedStory);
            var response = this.client.Execute(editRequest);
            var jsonResponse = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "Should return status code Not Found (404)");
            Assert.That(jsonResponse.msg, Is.EquivalentTo("No spoilers..."));

        }

        [Test, Order(7)]

        public void DeleteNonExistingStory_ShouldReturnBadRequest()
        {
            var fakeStoryId = "384kd438";
            var deleteRequest = new RestRequest($"/api/Story/Delete/{fakeStoryId}", Method.Delete);
            var response = this.client.Execute(deleteRequest);
            var jsonResponse = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(jsonResponse.msg, Is.EquivalentTo("Unable to delete this story spoiler!"));
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            client?.Dispose();
        }
    }
}