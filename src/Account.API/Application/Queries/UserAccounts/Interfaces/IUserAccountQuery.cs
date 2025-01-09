using Account.API.Application.Models;
namespace Account.API.Application.Queries.UserAccounts.Interfaces;
public interface IUserAccountQuery
{
    Task<IEnumerable<UserAccountModel>> GetAllUsersAsync(UserAccountFilterModel filter);
    Task<UserAccountDetailModel> GetUserAccountByIdAsync(Guid id);
}