using Account.API.Application.Models;
using Account.API.Application.Commands.UserAccountCommands;
namespace Account.API.Application.CommandsValidations.ApplicationUserValidations;
public class UserAccountCreateCommandValidator :
    UserAccountBaseCommandValidator<ApplicationUserCreateCommand, UserAccountModel>
{
    public UserAccountCreateCommandValidator(IServiceProvider serviceProvider, ILogger<UserAccountCreateCommandValidator> logger)
    {
        ValidateUsernameExistence(serviceProvider, logger);
        ValidateFirstName();
        ValidateLastName();
    }
}