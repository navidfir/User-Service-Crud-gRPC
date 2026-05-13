namespace Application.Users.DTOs;

public class UserDto
{
    public Guid UserId { get; set; }

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string NationalCode { get; set; } = default!;

    public DateTime? BirthDate { get; set; }

    public Guid Version { get; set; }
}