


using System.ComponentModel.DataAnnotations;

public sealed class User
{
    public Guid Id {get; set;} = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Name {get; set;} = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Email {get; set;} = string.Empty;

    [Required]
    public string PasswordHash {get; set;} = string.Empty;

    public UserStatus Status {get; set;} = UserStatus.Unverified;

    public DateTimeOffset CreatedAt {get; set;} = DateTimeOffset.UtcNow;

    public DateTimeOffset? LastLoginAt {get; set;}
}