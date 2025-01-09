using Account.API.Application.Commands.UserAccountCommands;
using Account.API.Application.CommandsValidations.ApplicationUserValidations;
namespace Account.API.Application.CommandsValidations.UserAccountValidations;

public class UserAccountDeleteCommandValidator : UserAccountBaseCommandValidator<UserAccountDeleteCommand, bool>
{
    public UserAccountDeleteCommandValidator(IServiceProvider serviceProvider, ILogger<UserAccountDeleteCommandValidator> logger)
    {
        ValidateId(serviceProvider, logger);
    }
}