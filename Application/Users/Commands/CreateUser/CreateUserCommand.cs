using Application.Users.DTOs;
using MediatR;

namespace Application.Users.Commands.CreateUser;

public record CreateUserCommand(
    string FirstName,
    string LastName,
    string NationalCode,
    DateTime? BirthDate
) : IRequest<UserDto>;