using Application.Users.DTOs;
using MediatR;

namespace Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string NationalCode,
    DateTime? BirthDate,
    Guid Version
) : IRequest<UserDto>;