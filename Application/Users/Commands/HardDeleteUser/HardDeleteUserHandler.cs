using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands.HardDeleteUser;

public class HardDeleteUserHandler : IRequestHandler<HardDeleteUserCommand>
{
    private readonly IApplicationDbContext _db;

    public HardDeleteUserHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(HardDeleteUserCommand request, CancellationToken ct)
    {
        var user = await _db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.UserId == request.UserId, ct);

        if (user == null)
            throw new NotFoundException("User not found");

        _db.Users.Remove(user);

        await _db.SaveChangesAsync(ct);
    }
}