using MediatR;
using Account.Domain.Constants;
using Account.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Account.API.Application.Models;
using Account.Domain.Aggregates.UserAggregate;
using Account.API.Application.Commands.UserAccountCommands.Commands;
namespace Account.API.Application.Commands.UserAccountCommands;

public class ApplicationUserCreateCommand : UserAccountBaseCommand<UserAccountModel> { }

public class UserAccountCreateCommandHandler(
        UserManager<IdentityUser> userManager,
        IApplicationUserRepository applicationUserRepository,
        ILogger<UserAccountCreateCommandHandler> applicationUserCreateCommandHandlerLogger) 
    : IRequestHandler<ApplicationUserCreateCommand, UserAccountModel>
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly IApplicationUserRepository _applicationUserRepository = applicationUserRepository;
    private readonly ILogger<UserAccountCreateCommandHandler> 
        _applicationUserCreateCommandHandlerLogger = applicationUserCreateCommandHandlerLogger;

    public async Task<UserAccountModel> Handle(ApplicationUserCreateCommand command, CancellationToken cancellationToken)
    {
        IdentityUser identityUser = await CreateIdentityUserAsync(command);

        await AssignUserRoleAsync(identityUser, command.Role);

        ApplicationUser applicationUser = new(identityUser.Id, command.FirstName, command.LastName) 
        {
            Creator = string.Empty
        };

        await _applicationUserRepository.AddSingleAsync(applicationUser, cancellationToken);

        return new UserAccountModel()
        {
            Id = applicationUser.ID,
            LastName = applicationUser.LastName,
            FirstName = applicationUser.FirstName,
            Username = applicationUser.IdentityUser.UserName ?? string.Empty,
            Role = command.Role
        };
    }

    private async Task<IdentityUser> CreateIdentityUserAsync(ApplicationUserCreateCommand command)
    {
        IdentityUser identityUser = new() { Email = command.Username, UserName = command.Username };
        
        _applicationUserCreateCommandHandlerLogger.LogWarning("------Register a new IdentityUser {0}", @identityUser);
        
        IdentityResult identityResult = await _userManager.CreateAsync(identityUser, command.Password);

        return identityResult.Succeeded ? identityUser : throw new DomainException(ErrorConstant.USER_COULD_NOT_BE_REGISTRED);
    }

    private async Task AssignUserRoleAsync(IdentityUser identityUser, string role)
    {
        IdentityResult identityResult = await _userManager.AddToRoleAsync(identityUser, role);

        if (!identityResult.Succeeded) throw new DomainException(ErrorConstant.ROLE_COULD_NOT_BE_ADDED);
    }
}