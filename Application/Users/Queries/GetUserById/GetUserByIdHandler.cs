using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Users.DTOs;
using Application.Users.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries.GetUserById;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IApplicationDbContext _db;

    public GetUserByIdHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == request.UserId, ct);

        if (user == null)
            throw new NotFoundException("User not found");

        return user.ToDto();
    }
}