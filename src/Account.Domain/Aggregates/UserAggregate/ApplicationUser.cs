using Account.Domain.Seedwork;
using Microsoft.AspNetCore.Identity;
namespace Account.Domain.Aggregates.UserAggregate;
public class ApplicationUser : AggregateRoot<string>
{
    public required string Creator { get; set; }
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public bool IsDeleted { get; set; } = default!;
    public IdentityUser IdentityUser { get; private set; } = default!;

    public ApplicationUser() { }

    public ApplicationUser(string userId, string firstName, string lastName)
    {
        ID = userId;
        FirstName = firstName;
        LastName = lastName;        
    }

    public void UpdateUser(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public void DeleteUser()
    {
        throw new NotImplementedException();
    }
}