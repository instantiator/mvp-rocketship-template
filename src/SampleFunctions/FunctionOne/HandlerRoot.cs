using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;

namespace FunctionOne;

public class HandlerRoot
{
    public static Dictionary<string, string> ALL_HEADERS = new Dictionary<string, string>()
    {
        { "Access-Control-Allow-Headers", "Content-Type,Authorization" },
        { "Access-Control-Allow-Origin", "https://localhost:5001" },
        { "Access-Control-Allow-Methods", "OPTIONS,GET,POST" },
        { "Access-Control-Allow-Credentials", "true"}
    };

    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest request)
    {
        var sub = request.RequestContext.Authorizer?.Claims["sub"];

        return new APIGatewayProxyResponse()
        {
            StatusCode = 200,
            Body = $"ok; sub={sub ?? "(null)"}",
            Headers = ALL_HEADERS
        };
    }
}
