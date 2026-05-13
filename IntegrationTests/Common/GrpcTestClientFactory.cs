using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using User;

namespace IntegrationTests.Common;

public static class GrpcTestClientFactory
{
    public static UserService.UserServiceClient Create(WebApplicationFactory<Program> factory)
    {
        var client = factory.CreateDefaultClient();

        var channel = GrpcChannel.ForAddress(client.BaseAddress!, new GrpcChannelOptions
        {
            HttpClient = client
        });

        return new UserService.UserServiceClient(channel);
    }
}