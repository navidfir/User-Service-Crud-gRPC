using Api.Protos;
using Application.Users.Commands.CreateUser;
using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.HardDeleteUser;
using Application.Users.Commands.UpdateUser;
using Application.Users.Queries.GetUserById;
using Application.Users.Queries.GetUsers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;

namespace Api.Services;

public class UserGrpcService : UserService.UserServiceBase
{
    private readonly IMediator _mediator;

    public UserGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<UserResponse> CreateUser(
        CreateUserRequest request,
        ServerCallContext context)
    {
        var result = await _mediator.Send(
            new CreateUserCommand(
                request.FirstName,
                request.LastName,
                request.NationalCode,
                request.BirthDate?.ToDateTime()));

        return MapUser(result);
    }

    public override async Task<UserResponse> UpdateUser(
        UpdateUserRequest request,
        ServerCallContext context)
    {
        var result = await _mediator.Send(
            new UpdateUserCommand(
                Guid.Parse(request.UserId),
                request.FirstName,
                request.LastName,
                request.NationalCode,
                request.BirthDate?.ToDateTime(),
                Guid.Parse(request.Version)));

        return MapUser(result);
    }

    public override async Task<UserResponse> GetUserById(
        GetUserByIdRequest request,
        ServerCallContext context)
    {
        var result = await _mediator.Send(
            new GetUserByIdQuery(
                Guid.Parse(request.UserId)));

        return MapUser(result);
    }

    public override async Task<GetUsersResponse> GetUsers(
        GetUsersRequest request,
        ServerCallContext context)
    {
        var result = await _mediator.Send(
            new GetUsersQuery(
                request.Page,
                request.PageSize,
                request.Search,
                request.SortBy,
                request.Desc));

        var response = new GetUsersResponse
        {
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };

        response.Users.AddRange(
            result.Items.Select(MapUser));

        return response;
    }

    public override async Task<Empty> DeleteUser(
        DeleteUserRequest request,
        ServerCallContext context)
    {
        await _mediator.Send(
            new DeleteUserCommand(
                Guid.Parse(request.UserId)));

        return new Empty();
    }

    public override async Task<Empty> HardDeleteUser(
        DeleteUserRequest request,
        ServerCallContext context)
    {
        await _mediator.Send(
            new HardDeleteUserCommand(
                Guid.Parse(request.UserId)));

        return new Empty();
    }

    private static UserResponse MapUser(
        Application.Users.DTOs.UserDto dto)
    {
        return new UserResponse
        {
            UserId = dto.UserId.ToString(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            NationalCode = dto.NationalCode,
            Version = dto.Version.ToString(),

            BirthDate = dto.BirthDate.HasValue
                ? Timestamp.FromDateTime(
                    dto.BirthDate.Value.ToUniversalTime())
                : null
        };
    }
}