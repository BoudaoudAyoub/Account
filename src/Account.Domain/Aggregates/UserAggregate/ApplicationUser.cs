using Account.Domain.Seedwork;
using Microsoft.AspNetCore.Identity;
namespace Account.Domain.Aggregates.UserAggregate;
public class ApplicationUser : AggregateRoot<Guid>
{
    public required string Creator { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public int IsDeleted { get; set; } = 0;
    public IdentityUser IdentityUser { get; set; } = default!;

    public ApplicationUser() { }

    public ApplicationUser(string firstName, string lastName, string userName, string password)
    {
        FirstName = firstName;
        LastName = lastName;        
    }
}