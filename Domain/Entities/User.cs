using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class User
{
    public Guid UserId { get; set; }

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string NationalCode { get; set; } = default!;

    public DateTime? BirthDate { get; set; }

    [ConcurrencyCheck]
    public Guid Version { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
}