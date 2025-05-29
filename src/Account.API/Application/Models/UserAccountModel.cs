using System.ComponentModel.DataAnnotations;
namespace Account.API.Application.Models;

public class UserAccountModel : UserAccountBaseModel
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = default!;
}

public class UserAccountDetailModel : UserAccountModel 
{
    //TODO: add more deltails here
}

public class UserAcountUpdateRequest : UserAccountBaseModel
{
    //TODO: add more deltails here
}

public class UserAccountCreateRequest : UserAccountCredentialModel
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Role { get; set; }
}

public class UserAccountCredentialModel
{
    [EmailAddress]
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class UserAccountBaseModel
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string PhoneNumber { get; set; } = default!;
    public required string Role { get; set; }
}