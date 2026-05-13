using MediatR;

namespace Application.Users.Commands.HardDeleteUser;

public record HardDeleteUserCommand(Guid UserId) : IRequest;