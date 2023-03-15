using System;
using Amazon.Extensions.CognitoAuthentication;
using EndpointTests;

namespace EndpointTests
{
    [TestClass]
    public class CognitoAuthenticationTests : AbstractEndpointTests
    {
        [TestMethod]
        public void EnvironmentIsProvided()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(TestUserEmail));
            Assert.IsFalse(string.IsNullOrWhiteSpace(TestUserPassword));

            Assert.IsFalse(string.IsNullOrWhiteSpace(FunctionOneEndpoint));
            Assert.IsFalse(string.IsNullOrWhiteSpace(CognitoClientId));
            Assert.IsFalse(string.IsNullOrWhiteSpace(CognitoUserPoolId));
        }

        [TestMethod]
        public async Task CanGetIdTokenAsync()
        {
            var token = await GetIdTokenAsync();
            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
        }

        [TestMethod]
        public async Task CanGetAccessTokenAsync()
        {
            var token = await GetAccessTokenAsync();
            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
        }

        [TestMethod]
        public async Task IdAndAccessTokensDiffer()
        {
            var access = await GetAccessTokenAsync();
            var id = await GetIdTokenAsync();
            Assert.AreNotEqual(access, id);
        }

    }
}

