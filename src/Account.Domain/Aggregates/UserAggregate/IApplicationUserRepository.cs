using Account.Domain.Seedwork;
namespace Account.Domain.Aggregates.UserAggregate;
public interface IApplicationUserRepository : IRepository<ApplicationUser, string>
{
    Task<bool> DoesUserExistsByUserName(string username);
}