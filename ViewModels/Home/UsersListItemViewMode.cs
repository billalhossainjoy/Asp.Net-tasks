namespace Asp.Net_tasks.ViewModel.Home;


public sealed class UserListItemViewModel
{
    
    public Guid Id {get; set;} 
    public string Name {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
    public UserStatus Status {get; set;}

    public DateTimeOffset? LastLoginAt {get;set;}
}