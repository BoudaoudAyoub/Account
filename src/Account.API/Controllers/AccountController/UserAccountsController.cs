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
public record UserToken(string Token);

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
    public async Task<UserToken> Login(UserAccountCredentialModel userAccountCredentialModel) => new(await _userAccountQuery.LoginAsync(userAccountCredentialModel));

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

    [HttpGet]
    [ProducesResponseType(typeof(int), HttpResponseType.NoContent)]
    [ProducesResponseType(typeof(int), HttpResponseType.InternalServerError)]
    [ProducesResponseType(typeof(IEnumerable<UserAccountModel>), HttpResponseType.Ok)]
    public async Task<IEnumerable<UserAccountModel>> GetAllUsersAccountsAsync([FromBody] UserAccountFilterModel filter) => await _userAccountQuery.GetAllUsersAsync(filter);

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(int), HttpResponseType.BadRequest)]
    [ProducesResponseType(typeof(int), HttpResponseType.InternalServerError)]
    [ProducesResponseType(typeof(UserAccountDetailModel), HttpResponseType.Ok)]
    public async Task<UserAccountDetailModel> GetUserAccountByIdAsync(Guid id) => await _userAccountQuery.GetUserAccountByIdAsync(id);

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

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(bool), HttpResponseType.Ok)]
    [ProducesResponseType(typeof(int), HttpResponseType.BadRequest)]
    [ProducesResponseType(typeof(int), HttpResponseType.InternalServerError)]
    public async Task<bool> DeleteUserByIdAsync(Guid id) => await _mediator.Send(new UserAccountDeleteCommand(id.ToString()));
}