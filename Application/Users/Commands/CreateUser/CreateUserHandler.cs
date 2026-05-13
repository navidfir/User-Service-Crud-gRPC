using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Users.DTOs;
using Application.Users.Mappings;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands.CreateUser;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IApplicationDbContext _db;

    public CreateUserHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var exists = await _db.Users
            .AnyAsync(x => x.NationalCode == request.NationalCode, ct);

        if (exists)
            throw new AlreadyExistsException("NationalCode already exists");

        var user = new User
        {
            UserId = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            NationalCode = request.NationalCode,
            BirthDate = request.BirthDate,
            CreatedAtUtc = DateTime.UtcNow,
            Version = Guid.NewGuid()
        };

        await _db.Users.AddAsync(user, ct);

        await _db.SaveChangesAsync(ct);

        return user.ToDto();
    }
}