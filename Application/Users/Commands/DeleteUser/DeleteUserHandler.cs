using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands.DeleteUser;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IApplicationDbContext _db;

    public DeleteUserHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken ct)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(x => x.UserId == request.UserId, ct);

        if (user == null)
            throw new NotFoundException("User not found");

        user.IsDeleted = true;
        user.DeletedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
    }
}