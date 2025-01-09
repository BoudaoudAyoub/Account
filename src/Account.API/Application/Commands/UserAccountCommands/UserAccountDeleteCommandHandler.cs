using MediatR;
using Account.Domain.Aggregates.UserAggregate;
using Account.API.Application.Commands.UserAccountCommands.Commands;
namespace Account.API.Application.Commands.UserAccountCommands;
public class UserAccountDeleteCommand : UserAccountBaseCommand<bool> 
{
    public UserAccountDeleteCommand(string userId) =>  Id = userId;
}

public class UserAccountDeleteCommandHandler(
        IApplicationUserRepository applicationUserRepository,
        ILogger<UserAccountDeleteCommandHandler> userAccountDeleteCommandHandlerLogger)
    : IRequestHandler<UserAccountDeleteCommand, bool>
{
    private readonly IApplicationUserRepository _applicationUserRepository = applicationUserRepository;
    private readonly ILogger<UserAccountDeleteCommandHandler> _userAccountDeleteCommandHandlerLogger = userAccountDeleteCommandHandlerLogger;

    public async Task<bool> Handle(UserAccountDeleteCommand command, CancellationToken cancellationToken)
    {
        ApplicationUser applicationUser = await _applicationUserRepository.GetByIdAsync(command.Id, cancellationToken);

        if (applicationUser.IsDeleted)
        {
            _userAccountDeleteCommandHandlerLogger.LogInformation("---Soft delete for the user Id: {0}---", command.Id);
            _applicationUserRepository.RemoveSingle(applicationUser);
        }
        else
        {
            _userAccountDeleteCommandHandlerLogger.LogInformation("---Deleting the user Id: {0} permanently---", command.Id);
            applicationUser.DeleteUser();
            _applicationUserRepository.UpdateSingle(applicationUser);
        }

        return true;
    }
}
