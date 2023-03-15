using EndpointTests;
using RestSharp;

namespace Saas.EndpointTests;

[TestClass]
public class RootEndpointTests : AbstractEndpointTests
{
    [TestMethod]
    public async Task RootNoAuthEndpointDoesNotRequireToken()
    {
        var client = new RestClient(FunctionOneEndpoint_NoAuth);
        var request = new RestRequest();
        var result = await client.GetAsync(request);

        Assert.IsTrue(result.IsSuccessStatusCode);
        Assert.IsNotNull(result.Content);
        Assert.AreEqual("ok; sub=(null)", result.Content);
    }

    [TestMethod]
    public async Task RootEndpointAcceptsAccessToken()
    {
        var token = await GetAccessTokenAsync();
        var client = new RestClient(FunctionOneEndpoint);
        client.AddDefaultHeader("Authorization", $"Bearer {token}");

        var request = new RestRequest();
        var result = await client.GetAsync(request);

        Assert.IsTrue(result.IsSuccessStatusCode);
        Assert.IsNotNull(result.Content);
        Assert.IsTrue(result.Content!.StartsWith($"ok; sub="));

        var sub = result.Content!.Split("=")[1];
        Assert.IsFalse(string.IsNullOrWhiteSpace(sub));
        Assert.IsTrue(sub.Length > 1);
        Assert.AreNotEqual("(null)", sub);
    }

    [TestMethod]
    public async Task RootEndpointDoesNotAcceptIdToken()
    {
        await Assert.ThrowsExceptionAsync<HttpRequestException>(async () =>
        {
            var token = await GetIdTokenAsync();
            var client = new RestClient(FunctionOneEndpoint);
            client.AddDefaultHeader("Authorization", $"Bearer {token}");
            var request = new RestRequest();
            await client.GetAsync(request);
        });
    }
}
