using EndpointTests;
using RestSharp;

namespace EndpointTests;

[TestClass]
public class RootEndpointTests : AbstractEndpointTests
{
    [TestMethod]
    public async Task GetRootEndpointDoesNotRequireToken()
    {
        var client = new RestClient(FunctionOneEndpoint);
        var request = new RestRequest();
        var result = await client.GetAsync(request);

        Assert.IsTrue(result.IsSuccessStatusCode);
        Assert.IsNotNull(result.Content);
        Assert.AreEqual("ok; sub=(null)", result.Content);
    }

    [TestMethod]
    public async Task OptionsRootEndpointDoesNotRequireToken()
    {
        var client = new RestClient(FunctionOneEndpoint);
        var request = new RestRequest();
        var result = await client.OptionsAsync(request);

        Assert.IsTrue(result.IsSuccessStatusCode);
        Assert.IsNotNull(result.Content);
        Assert.AreEqual("ok; sub=(null)", result.Content);
    }

}
