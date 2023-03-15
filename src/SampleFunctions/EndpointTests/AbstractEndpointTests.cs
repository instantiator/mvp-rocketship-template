using System;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using System.Collections;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using System.Text.Json;

namespace EndpointTests
{
    public abstract class AbstractEndpointTests
    {
        protected AbstractEndpointTests()
        {
            var environment = Environment.GetEnvironmentVariables();
            var env = environment.Cast<DictionaryEntry>().ToDictionary(e => (string)e.Key, e => (string?)e.Value);

            TestUserEmail = env["TEST_USER_EMAIL"]!;
            TestUserPassword = env["TEST_USER_PASSWORD"]!;

            FunctionOneEndpoint = env["OUT_FunctionOneEndpoint"]!;
            CognitoClientId = env["OUT_CognitoClientId"]!;
            CognitoUserPoolId = env["OUT_UserPoolId"]!;
        }

        protected string TestUserEmail { get; private set; }
        protected string TestUserPassword { get; private set; }
        protected string CognitoClientId { get; private set; }
        protected string CognitoUserPoolId { get; private set; }

        protected string FunctionOneEndpoint { get; private set; }
        protected string FunctionOneEndpoint_NoAuth => FunctionOneEndpoint + "/noauth";
        protected string FunctionOneNotesEndpoint => FunctionOneEndpoint + "/notes";
        protected string FunctionOneNotesEndpoint_NoAuth => FunctionOneEndpoint + "/notes/noauth";

        protected async Task<string> GetIdTokenAsync()
            => await GetIdTokenAsync(CognitoUserPoolId, CognitoClientId, TestUserEmail, TestUserPassword);

        protected async Task<string> GetAccessTokenAsync()
            => await GetAccessTokenAsync(CognitoUserPoolId, CognitoClientId, TestUserEmail, TestUserPassword);

        protected async Task<string> GetIdTokenAsync(string pool, string client, string username, string password)
        {
            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials());
            CognitoUserPool userPool = new CognitoUserPool(pool, client, provider); ;
            CognitoUser user = new CognitoUser(username, client, userPool, provider);
            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest() { Password = password };
            AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
            return authResponse.AuthenticationResult.IdToken;
        }

        protected async Task<string> GetAccessTokenAsync(string pool, string client, string username, string password)
        {
            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials());
            CognitoUserPool userPool = new CognitoUserPool(pool, client, provider); ;
            CognitoUser user = new CognitoUser(username, client, userPool, provider);
            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest() { Password = password };
            AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
            return authResponse.AuthenticationResult.AccessToken;
        }
    }
}
