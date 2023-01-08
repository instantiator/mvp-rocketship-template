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
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest request)
    {
        var tableName = Environment.GetEnvironmentVariable("NOTES_TABLE");
        var region = RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("AWS_REGION"));

        Console.WriteLine($"Region: {region}");
        Console.WriteLine($"Table:  {tableName}");

        AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig()
        {
            RegionEndpoint = region,
        };
        var dbClient = new AmazonDynamoDBClient(clientConfig);

        string? output;
        switch (request.HttpMethod.ToLower()) 
        {
            case "get":
                var scanRequest = new ScanRequest { TableName = tableName };
                var response = await dbClient.ScanAsync(scanRequest);
                output = $"{tableName}: {response.Count}";
                break;
            case "post":
                var id = Guid.NewGuid().ToString();
                var putRequest = new PutItemRequest
                {
                    TableName = tableName,
                    Item = new Dictionary<string, AttributeValue>()
                    {
                        { "Id", new AttributeValue { S = id }},
                        { "Note", new AttributeValue { S = $"Note with id: {id}" }},
                    }
                };
                var putResponse = await dbClient.PutItemAsync(putRequest);
                output = $"put code {putResponse.HttpStatusCode}";
                break;
            default:
                throw new ArgumentException($"Method {request.HttpMethod} not implemented.");
        }

        return new APIGatewayProxyResponse()
        {
            StatusCode = 200,
            Body = output ?? "(null)"
        };
    }
}
