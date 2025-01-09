using Microsoft.EntityFrameworkCore;
using Account.Domain.Aggregates.UserAggregate;
namespace Account.Infrastructure.Repositories;
public class ApplicationUserRepository(AccountDbContext accountDbContext) : 
    Repository<ApplicationUser, string>(accountDbContext), 
    IApplicationUserRepository
{
    public async Task<bool> DoesUserExistsByUserName(string username)
        => await _accountDbContext.ApplicationUsers
        .AnyAsync(apu => username.ToLower()
        .Equals(!string.IsNullOrEmpty(apu.IdentityUser.UserName) 
            ? apu.IdentityUser.UserName.ToLower() 
            : string.Empty));
}