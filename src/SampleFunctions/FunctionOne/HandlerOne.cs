using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.APIGatewayEvents;

namespace FunctionOne;

public class HandlerOne
{
    public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest request)
    {
        return null; // TODO
    }
}

