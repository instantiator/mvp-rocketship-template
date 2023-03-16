using System;
using System.Runtime.Intrinsics.X86;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Runtime;

namespace FunctionOne;

public class HandlerNotes
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
        Console.WriteLine($"HTTP Method: {request.HttpMethod}");

        var tableName = Environment.GetEnvironmentVariable("NOTES_TABLE");
        var region = RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("AWS_REGION"));

        Console.WriteLine($"Region: {region}");
        Console.WriteLine($"Table:  {tableName}");

        AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig()
        {
            RegionEndpoint = region,
        };
        var dbClient = new AmazonDynamoDBClient(clientConfig);

        var sub = request.RequestContext.Authorizer?.Claims["sub"];

        string? output;
        switch (request.HttpMethod.ToLower()) 
        {
            case "get":
                var scanRequest = new ScanRequest { TableName = tableName };
                var response = await dbClient.ScanAsync(scanRequest);
                output = $"Notes in {tableName}: {response.Count}";
                break;
            case "post":
                var id = Guid.NewGuid().ToString();
                var putRequest = new PutItemRequest
                {
                    TableName = tableName,
                    Item = new Dictionary<string, AttributeValue>()
                    {
                        { "Id", new AttributeValue { S = id }},
                        { "Note", new AttributeValue { S = $"Note with id: {id}, created by subject with id: {sub}" }},
                    }
                };
                var putResponse = await dbClient.PutItemAsync(putRequest);
                output = $"put db code {putResponse.HttpStatusCode}";
                break;
            case "options":
                output = "Endpoint options requested.";
                break;
            default:
                throw new ArgumentException($"Method {request.HttpMethod} not implemented.");
        }

        return new APIGatewayProxyResponse()
        {
            StatusCode = 200,
            Body = output ?? "(null)",
            Headers = ALL_HEADERS
        };
    }
}
