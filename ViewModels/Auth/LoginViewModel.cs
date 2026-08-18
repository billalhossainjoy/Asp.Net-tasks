using System.ComponentModel.DataAnnotations;

namespace Asp.Net_tasks.ViewModel.Auth;

public sealed class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email {get;set;} = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password {get; set;} = string.Empty;

    public bool IsRememberMe {get; set;} = false;
};