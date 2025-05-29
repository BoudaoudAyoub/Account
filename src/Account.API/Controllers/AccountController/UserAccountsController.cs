using MediatR;
using AutoMapper;
using Account.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Account.Domain.Exceptions;
using Account.API.Application.Models;
using Account.SharedKernel.HttpReponses;
using Account.API.Application.Commands.UserAccountCommands;
using Account.API.Application.Queries.UserAccounts.Interfaces;
using Microsoft.AspNetCore.Authorization;
namespace Account.API.Controllers.AccountController;
public class UserAccountsController(ILogger<UserAccountsController> logger,
    IMediator mediator,
    IMapper mapper,
    IUserAccountQuery userAccountQuery)
    : BaseController
{
    private readonly IMapper _mapper = mapper;
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<UserAccountsController> _logger = logger;
    private readonly IUserAccountQuery _userAccountQuery = userAccountQuery;

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(int), HttpResponseType.Ok)]
    [ProducesResponseType(typeof(int), HttpResponseType.NotFound)]
    public async Task<UserToken> Login(UserAccountCredentialModel userAccountCredentialModel)
    => new(await _userAccountQuery.LoginAsync(userAccountCredentialModel));

    public record UserToken(string Token);

    /// <summary>
    /// Creates a new user account
    /// </summary>
    /// <param name="request">An object contains the information to be stored</param>
    /// <returns>An object contains the created user account information</returns>
    /// <exception cref="DomainException">A exception will be thrown if the request model is null</exception>
    [HttpPost]
    [ProducesResponseType(typeof(int), HttpResponseType.Created)]
    [ProducesResponseType(typeof(int), HttpResponseType.BadRequest)]
    [ProducesResponseType(typeof(int), HttpResponseType.InternalServerError)]
    public async Task<UserAccountModel> CreateUserAccount(UserAccountCreateRequest request)
    {
        if (request is null)
        {
            _logger.LogError("Failed to create user. The {0} object cannot be null", request);
            throw new DomainException(ErrorConstant.SYSTEM_INVALID_DATA_MESSAGE);
        }

        ApplicationUserCreateCommand command = _mapper.Map<ApplicationUserCreateCommand>(request);
        return await _mediator.Send(command);
    }

    /// <summary>
    /// Get all users acounts
    /// </summary>
    /// <returns>An enumerable of user model contains all users accounts information</returns>
    [HttpGet]
    [ProducesResponseType(typeof(int), HttpResponseType.NoContent)]
    [ProducesResponseType(typeof(int), HttpResponseType.InternalServerError)]
    [ProducesResponseType(typeof(IEnumerable<UserAccountModel>), HttpResponseType.Ok)]
    public async Task<IEnumerable<UserAccountModel>> GetAllUsersAccountsAsync([FromBody] UserAccountFilterModel filter)
    => await _userAccountQuery.GetAllUsersAsync(filter);

    /// <summary>
    /// Get user account by ID
    /// </summary>
    /// <param name="id">A unique identifier of a user account</param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(int), HttpResponseType.BadRequest)]
    [ProducesResponseType(typeof(int), HttpResponseType.InternalServerError)]
    [ProducesResponseType(typeof(UserAccountDetailModel), HttpResponseType.Ok)]
    public async Task<UserAccountDetailModel> GetUserAccountByIdAsync(Guid id)
        => await _userAccountQuery.GetUserAccountByIdAsync(id);

    /// <summary>
    /// Update a user account details
    /// </summary>
    /// <param name="id">A unique identifier</param>
    /// <param name="request">an object represents the data to be upated</param>
    /// <returns>AN object represents the updated user account</returns>
    /// <exception cref="DomainException">A exception will be thrown if the request model is null</exception>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(int), HttpResponseType.BadRequest)]
    [ProducesResponseType(typeof(int), HttpResponseType.InternalServerError)]
    [ProducesResponseType(typeof(UserAccountDetailModel), HttpResponseType.Ok)]
    public async Task<UserAccountDetailModel> UpdateUserAccountByIdAsync(Guid id, [FromBody] UserAcountUpdateRequest request)
    {
        if (request is null)
        {
            _logger.LogError("Failed to update user. User ID: {0}. Request object {1} cannot be null", id, request);
            throw new DomainException(ErrorConstant.SYSTEM_INVALID_DATA_MESSAGE);
        }

        UserAccountUpdateCommand command = _mapper.Map<UserAccountUpdateCommand>(request);
        command.Id = id.ToString();
        return await _mediator.Send(command);
    }

    /// <summary>
    /// Delete a user softly then permanently if needed
    /// </summary>
    /// <param name="id">A unique identifier of a user</param>
    /// <returns>A boolean indicates whether the operation succeeded or not</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(bool), HttpResponseType.Ok)]
    [ProducesResponseType(typeof(int), HttpResponseType.BadRequest)]
    [ProducesResponseType(typeof(int), HttpResponseType.InternalServerError)]
    public async Task<bool> DeleteUserByIdAsync(Guid id)
        => await _mediator.Send(new UserAccountDeleteCommand(id.ToString()));
}