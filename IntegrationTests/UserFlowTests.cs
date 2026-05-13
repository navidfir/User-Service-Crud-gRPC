using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IntegrationTests.Common;
using User;
using Xunit;

namespace IntegrationTests;

public class UserFlowTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly UserService.UserServiceClient _client;

    public UserFlowTests(CustomWebApplicationFactory factory)
    {
        _client = GrpcTestClientFactory.Create(factory);
    }

    [Fact]
    public async Task Full_User_Flow_Should_Work()
    {
        // CREATE

        var created = await _client.CreateUserAsync(
            new CreateUserRequest
            {
                FirstName = "Navid",
                LastName = "FN",
                NationalCode = "999999",
                BirthDate = Timestamp.FromDateTime(DateTime.UtcNow)
            }).ResponseAsync;

        created.FirstName.Should().Be("Navid");

        // DUPLICATE

        Func<Task> duplicate = async () =>
        {
            await _client.CreateUserAsync(
                new CreateUserRequest
                {
                    FirstName = "Duplicate",
                    LastName = "User",
                    NationalCode = "999999"
                }).ResponseAsync;
        };

        var duplicateException =
            await Assert.ThrowsAsync<RpcException>(duplicate);

        duplicateException.StatusCode
            .Should()
            .Be(StatusCode.AlreadyExists);

        // GET USER

        var fetched = await _client.GetUserByIdAsync(
            new GetUserByIdRequest
            {
                UserId = created.UserId
            }).ResponseAsync;

        fetched.FirstName.Should().Be("Navid");

        // GET ALL

        var users = await _client.GetUsersAsync(
            new GetUsersRequest
            {
                Page = 1,
                PageSize = 10,
                Search = "Nav"
            }).ResponseAsync;

        users.TotalCount.Should().BeGreaterThan(0);

        // UPDATE SUCCESS

        var updated = await _client.UpdateUserAsync(
            new UpdateUserRequest
            {
                UserId = created.UserId,
                FirstName = "Updated",
                LastName = "Updated",
                NationalCode = "999999",
                Version = created.Version
            }).ResponseAsync;

        updated.FirstName.Should().Be("Updated");

        // UPDATE WITH OLD VERSION

        Func<Task> conflict = async () =>
        {
            await _client.UpdateUserAsync(
                new UpdateUserRequest
                {
                    UserId = created.UserId,
                    FirstName = "Conflict",
                    LastName = "Conflict",
                    NationalCode = "999999",
                    Version = created.Version
                }).ResponseAsync;
        };

        var conflictException =
            await Assert.ThrowsAsync<RpcException>(conflict);

        conflictException.StatusCode
            .Should()
            .Be(StatusCode.Aborted);

        // SOFT DELETE

        var softDelete = await _client.DeleteUserAsync(
            new DeleteUserRequest
            {
                UserId = created.UserId
            }).ResponseAsync;
    }
}