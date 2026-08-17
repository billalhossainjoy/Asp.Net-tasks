using System.ComponentModel.DataAnnotations;

namespace Asp.Net_task3.ViewModel.Auth;

public sealed class RegisterViewModel
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password and confirm Password do no match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public bool Terms {get;set;}
}