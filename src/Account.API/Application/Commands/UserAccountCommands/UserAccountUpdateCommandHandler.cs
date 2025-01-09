using MediatR;
using Microsoft.AspNetCore.Identity;
using Account.API.Application.Models;
using Account.Domain.Aggregates.UserAggregate;
using Account.API.Application.Commands.UserAccountCommands.Commands;
using Account.Domain.Exceptions;
using Account.Domain.Constants;

namespace Account.API.Application.Commands.UserAccountCommands;

public class UserAccountUpdateCommand : UserAccountBaseCommand<UserAccountDetailModel> { }

public class UserAccountUpdateCommandHandler(
    ILogger<UserAccountUpdateCommandHandler> logger,
    UserManager<IdentityUser> identityUserManager,
    IApplicationUserRepository applicationUserRepository) 
    : IRequestHandler<UserAccountUpdateCommand, UserAccountDetailModel>
{
    private readonly ILogger<UserAccountUpdateCommandHandler> _userAccountUpdateCommandHandlerlogger = logger;
    private readonly IApplicationUserRepository _applicationUserRepository = applicationUserRepository;
    private readonly UserManager<IdentityUser> _identityUserManager = identityUserManager;

    public async Task<UserAccountDetailModel> Handle(UserAccountUpdateCommand command, CancellationToken cancellationToken)
    {
        _userAccountUpdateCommandHandlerlogger.LogWarning("------Update user: {0} - {1}", command.Id, command.Username);

        ApplicationUser applicationUser = await _applicationUserRepository.GetByIdAsync(command.Id, cancellationToken);

        await UpdateIdentityUser(applicationUser.ID, command);

        applicationUser.UpdateUser(command.FirstName, command.LastName);

        _applicationUserRepository.UpdateSingle(applicationUser);

        return new UserAccountDetailModel()
        {
            Role = string.Empty,
            Id = applicationUser.ID,
            LastName = applicationUser.LastName,
            FirstName = applicationUser.FirstName,
            Username = applicationUser.IdentityUser.UserName ?? string.Empty,
            PhoneNumber = applicationUser.IdentityUser.PhoneNumber ?? string.Empty
        };
    }

    private async Task UpdateIdentityUser(string id, UserAccountUpdateCommand command)
    {
        IdentityUser identityUser = await _identityUserManager.FindByIdAsync(id) 
            ?? throw new DomainException(ErrorConstant.SYSTEM_INVALID_DATA_MESSAGE);

        identityUser.PhoneNumber = command.PhoneNumber;

        await _identityUserManager.UpdateAsync(identityUser);

        //TODO: updates other properties if needed.
    }
}