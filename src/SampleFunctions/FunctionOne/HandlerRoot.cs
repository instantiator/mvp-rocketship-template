using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;

namespace FunctionOne;

public class HandlerRoot
{
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest request)
    {
        return new APIGatewayProxyResponse()
        {
            StatusCode = 200,
            Body = "ok"
        };
    }
}
