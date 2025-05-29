using Account.API.Application.Models;
namespace Account.API.Application.Queries.UserAccounts.Interfaces;
public interface IUserAccountQuery
{
    Task<string> LoginAsync(UserAccountCredentialModel userAccountCredentialModel);
    Task<IEnumerable<UserAccountModel>> GetAllUsersAsync(UserAccountFilterModel filter);
    Task<UserAccountDetailModel> GetUserAccountByIdAsync(Guid id);
}