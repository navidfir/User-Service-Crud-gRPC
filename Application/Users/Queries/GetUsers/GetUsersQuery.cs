using Application.Common.Models;
using Application.Users.DTOs;
using MediatR;

namespace Application.Users.Queries.GetUsers;

public record GetUsersQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    string? SortBy = null,
    bool Desc = false
) : IRequest<PagedResult<UserDto>>;