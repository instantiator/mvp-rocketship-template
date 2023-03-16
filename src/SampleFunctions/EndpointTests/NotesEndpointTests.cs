using System;
using RestSharp;

namespace EndpointTests
{
	[TestClass]
	public class NotesEndpointTests : AbstractEndpointTests
	{

        [TestMethod]
        public async Task GetNotesEndpointAcceptsAccessToken()
        {
            var token = await GetAccessTokenAsync();
            var client = new RestClient(FunctionOneNotesEndpoint);
            client.AddDefaultHeader("Authorization", $"Bearer {token}");

            var request = new RestRequest();
            var result = await client.GetAsync(request);

            Assert.IsTrue(result.IsSuccessStatusCode);
            Assert.IsNotNull(result.Content);
            Assert.IsTrue(result.Content!.StartsWith($"Notes in"));

            var count = int.Parse(result.Content!.Split(":")[1].Trim());
        }

        [TestMethod]
        public async Task GetNotesEndpointDoesNotAcceptIdToken()
        {
            await Assert.ThrowsExceptionAsync<HttpRequestException>(async () =>
            {
                var token = await GetIdTokenAsync();
                var client = new RestClient(FunctionOneNotesEndpoint);
                client.AddDefaultHeader("Authorization", $"Bearer {token}");
                var request = new RestRequest();
                await client.GetAsync(request);
            });
        }

        [TestMethod]
        public async Task OptionsNotesEndpointDoesNotRequireToken()
        {
            var client = new RestClient(FunctionOneNotesEndpoint);
            var request = new RestRequest();
            var result = await client.OptionsAsync(request);

            Assert.IsTrue(result.IsSuccessStatusCode);
        }

    }
}

