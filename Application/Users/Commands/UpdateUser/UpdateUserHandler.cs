using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Users.DTOs;
using Application.Users.Mappings;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<UpdateUserHandler> _logger;

    public UpdateUserHandler(
        IApplicationDbContext db,
        ILogger<UpdateUserHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<UserDto> Handle(
        UpdateUserCommand request,
        CancellationToken ct)
    {
        await using var transaction =
            await _db.BeginTransactionAsync(ct);

        try
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(
                    x => x.UserId == request.UserId,
                    ct);

            if (user is null)
                throw new NotFoundException("User not found");

            // optimistic concurrency check
            if (user.Version != request.Version)
                throw new ConcurrencyException(
                    "Concurrency conflict detected");

            // uniqueness validation
            var nationalCodeExists = await _db.Users
                .AnyAsync(x =>
                        x.NationalCode == request.NationalCode &&
                        x.UserId != request.UserId,
                    ct);

            if (nationalCodeExists)
                throw new AlreadyExistsException(
                    "NationalCode already exists");

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.NationalCode = request.NationalCode;
            user.BirthDate = request.BirthDate;
            user.Version = Guid.NewGuid();

            await _db.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);

            _logger.LogInformation(
                "User updated successfully {UserId}",
                user.UserId);

            return user.ToDto();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await transaction.RollbackAsync(ct);

            _logger.LogWarning(
                ex,
                "EF concurrency conflict for user {UserId}",
                request.UserId);

            throw new ConcurrencyException(
                "Database concurrency conflict");
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync(ct);

            _logger.LogError(
                ex,
                "Database update error for user {UserId}",
                request.UserId);

            throw;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}