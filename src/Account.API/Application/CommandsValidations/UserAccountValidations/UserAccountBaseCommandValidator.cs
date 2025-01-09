using FluentValidation;
using Account.Domain.Constants;
using Account.Domain.Aggregates.UserAggregate;
using Account.API.Application.Commands.UserAccountCommands.Commands;
namespace Account.API.Application.CommandsValidations.ApplicationUserValidations;

public class UserAccountBaseCommandValidator<TRequest, TResponse>
    : AbstractValidator<TRequest> where TRequest : UserAccountBaseCommand<TResponse>
{
    public UserAccountBaseCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
    }

    public void ValidateUsernameExistence(IServiceProvider serviceProvider, ILogger<object> logger)
        => RuleFor(command => command).Custom((command, context) =>
        {
            IApplicationUserRepository apu_repository = serviceProvider.GetRequiredService<IApplicationUserRepository>();
            if (apu_repository.DoesUserExistsByUserName(command.Username).Result)
            {
                logger.LogWarning("------The username {@Name} already exists", command.Username);
                context.AddFailure(ErrorConstant.USER_ALREADY_EXISTS);
            }
        });

    public void ValidateId(IServiceProvider serviceProvider, ILogger<object> logger)
    => RuleFor(command => command).Custom((command, context) =>
    {
        IApplicationUserRepository apu_repository = serviceProvider.GetRequiredService<IApplicationUserRepository>();
        if (apu_repository.DoesUserExistsByUserName(command.Username).Result)
        {
            logger.LogError("------The unique identifier: {@id} does not exists", command.Id);
            context.AddFailure(ErrorConstant.USER_ALREADY_EXISTS);
        }
    });

    public void ValidateFirstName() => RuleFor(command => command.FirstName)
        .NotNull().NotEmpty().WithMessage(ErrorConstant.USER_FIRST_NAME_MUST_NOT_BE_NULL_OR_EMPTY);

    public void ValidateLastName() => RuleFor(command => command.FirstName)
        .NotNull().NotEmpty().WithMessage(ErrorConstant.USER_LAST_NAME_MUST_NOT_BE_NULL_OR_EMPTY);
}