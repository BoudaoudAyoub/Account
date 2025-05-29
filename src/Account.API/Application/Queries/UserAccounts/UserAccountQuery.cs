using Dapper;
using Microsoft.Data.SqlClient;
using Account.Domain.Constants;
using Account.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Account.API.Application.Models;
using Account.API.Application.Queries.UserAccounts.Interfaces;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Account.API.Application.Queries.UserAccounts;

public class UserAccountQuery(
    ILogger<UserAccountQuery> logger,
    IConfiguration configuration,
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager) : IUserAccountQuery
{
    protected readonly ILogger<UserAccountQuery> _logger = logger;
    protected readonly UserManager<IdentityUser> _userManager = userManager;
    protected readonly SignInManager<IdentityUser> _signInManager = signInManager;
    protected readonly string _conectionStr = configuration.GetSection("AccountConnectionString")?.Value
        ?? throw new DomainException(ErrorConstant.CONNECTION_STRING_CANNOT_BE_NULL);

    public async Task<string> LoginAsync(UserAccountCredentialModel userAccountCredentialModel)
    {
        if (userAccountCredentialModel is null)
        {
            _logger.LogError("Failed to continue the process due to the object type of {0} cannot be null", typeof(UserAccountFilterModel));
            throw new DomainException($"{nameof(userAccountCredentialModel)} cannot be null");
        }

        IdentityUser identityUser = await _userManager.FindByEmailAsync(userAccountCredentialModel.Username) ??
            throw new DomainException(ErrorConstant.USERNAME_DOES_NOT_EXISTS, (int)HttpStatusCode.BadRequest);

        SignInResult signInResult = await _signInManager.PasswordSignInAsync(identityUser, userAccountCredentialModel.Password, false, true);

        if (!signInResult.Succeeded) throw new DomainException(ErrorConstant.INVALID_USER_CREDENTIALS, (int)HttpStatusCode.BadRequest);

        UserAccountDetailModel userDetails = await GetUserAccountByIdAsync(new(identityUser.Id));

        return GenerateUserToken(userDetails);
    }

    public async Task<IEnumerable<UserAccountModel>> GetAllUsersAsync(UserAccountFilterModel filter)
    {
        if (filter is null)
        {
            _logger.LogError("Failed to continue the process due to the object type of {0} cannot be null", typeof(UserAccountFilterModel));
            throw new DomainException($"{nameof(filter)} cannot be null");
        }

        _logger.LogInformation("----Start getting all users----");

        using var connection = new SqlConnection(_conectionStr);

        await connection.OpenAsync();

        IEnumerable<dynamic> users = await connection.QueryAsync<dynamic>(
            @"SELECT ap.[ID] as Id,
		             ap.[FirstName],
		             ap.[LastName],
		             apnu.[UserName],
		             apr.[Name] as Role
            FROM ApplicationUsers ap
            INNER JOIN AspNetUsers apnu on apnu.[Id] = ap.[ID]
            INNER JOIN AspNetUserRoles apur on apur.[UserId] = apnu.[Id]
            INNER JOIN AspNetRoles apr on apr.[Id] = apur.[RoleId]
            WHERE ap.[IsDeleted] = @deleted
            ORDER BY Id
            OFFSET @skip ROWS
            FETCH NEXT @take ROWS ONLY",
            new
            {
                filter.Deleted,
                skip = (filter.Page - 1) * filter.MaxRow,
                take = filter.MaxRow
            });

        await connection.CloseAsync();

        if (!users.Any()) return default!;

        return MapUsersToAccountModel(users);
    }

    public async Task<UserAccountDetailModel> GetUserAccountByIdAsync(Guid id)
    {
        _logger.LogInformation("----Start getting get a single user by the provided unique identifier : {0}----", id);

        using var connection = new SqlConnection(_conectionStr);

        await connection.OpenAsync();

        dynamic singleUser = await connection.QueryAsync<dynamic>(
            @"SELECT ap.[ID] as Id,
		             ap.[FirstName],
		             ap.[LastName],
		             apnu.[UserName],
		             apr.[Name] as Role
            FROM ApplicationUsers ap
            INNER JOIN AspNetUsers apnu on apnu.[Id] = ap.[ID]
            INNER JOIN AspNetUserRoles apur on apur.[UserId] = apnu.[Id]
            INNER JOIN AspNetRoles apr on apr.[Id] = apur.[RoleId]
            WHERE ap.[IsDeleted] = 0 AND ap.[id] = @id",
            new { id });

        if (singleUser is null) return default!;

        return new UserAccountDetailModel()
        {
            Id = singleUser[0].Id,
            Role = singleUser[0].Role,
            Username = singleUser[0].UserName,
            LastName = singleUser[0].LastName,
            FirstName = singleUser[0].FirstName,
        };
    }

    #region private methods

    private static IEnumerable<UserAccountModel> MapUsersToAccountModel(IEnumerable<dynamic> users)
    {
        return users.Select(user => new UserAccountModel()
        {
            Id = user.Id,
            Role = user.Role,
            Username = user.UserName,
            LastName = user.LastName,
            FirstName = user.FirstName,
        });
    }

    private string GenerateUserToken(UserAccountModel user)
    {
        // A Claim is an information about a subject
        // It gets encrypted into the JWT payload
        // Like here: the claim called "name" asserts that the user authenticating is called test@test.com
        List<Claim> authClaims =
        [
            new(ClaimTypes.Email, user.Username),
            new(ClaimTypes.Name, string.Join(" ", user.FirstName, user.LastName)),
            new Claim(ClaimTypes.Role, user.Role),
            // Jti is a JSON Token ID
            // The "jti" (JWT ID) claim provides a unique identifier for the JWT
            // The identifier value MUST be assigned in a manner that ensures that there is a negligible probability that the same value will be accidentally assigned to a different data object
            // The "jti" claim can be used to prevent the JWT from being replayed.
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        ];

        var authSigninKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JWT:Secret"] ?? string.Empty));

        var token = new JwtSecurityToken(
            issuer: configuration["JWT:ValidIssuer"], // Who created the token, for exapmle: A token valid for Google drive should not be accepted for Gmail, even if both of them have the same issuer, they’ll have different audiences.
            audience: configuration["JWT:ValidAudience"], // What is the target of this token; here, called User
            expires: DateTime.Now.AddDays(1),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256Signature));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    #endregion
}