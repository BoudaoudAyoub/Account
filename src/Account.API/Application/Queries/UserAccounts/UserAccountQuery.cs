using Dapper;
using Microsoft.Data.SqlClient;
using Account.Domain.Constants;
using Account.Domain.Exceptions;
using Account.API.Application.Models;
using Account.API.Application.Queries.UserAccounts.Interfaces;

namespace Account.API.Application.Queries.UserAccounts;

public class UserAccountQuery(ILogger<UserAccountQuery> logger, IConfiguration configuration) : IUserAccountQuery
{
    protected readonly ILogger<UserAccountQuery> _logger = logger;
    protected readonly string _conectionStr = configuration.GetSection("AccountConnectionString")?.Value
        ?? throw new DomainException(ErrorConstant.CONNECTION_STRING_CANNOT_BE_NULL);

    public async Task<IEnumerable<UserAccountModel>> GetAllUsersAsync(UserAccountFilterModel filter)
    {
        if (filter is null)
        {
            _logger.LogError($"Failed to continue the process due to the object type of {typeof(UserAccountFilterModel)} cannot be null");
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

        if (users.AsList().Count == 0) return default!;

        return MapUsersToAccountModel(users);
    }

    public async Task<UserAccountDetailModel> GetUserAccountByIdAsync(Guid id)
    {
        _logger.LogInformation($"----Start getting get a single user by the provided unique identifier : {id}----");

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

    #endregion
}