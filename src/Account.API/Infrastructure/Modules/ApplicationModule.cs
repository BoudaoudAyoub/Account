using System.Reflection;
using Account.API.Application.Behaviors;
using Account.API.Application.Commands.UserAccountCommands;
using Account.API.Application.CommandsValidations.ApplicationUserValidations;
using Autofac;
using FluentValidation;
using MediatR;
namespace Account.API.Infrastructure.Modules;

public class ApplicationModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Register IMediator interface
        builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

        // Register all the Command classes (they implement IRequestHandler) in assembly holding the Commands
        builder.RegisterAssemblyTypes(typeof(UserAccountCreateCommandHandler).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(IRequestHandler<,>));

        // Register the Command's Validators (Validators based on FluentValidation library)
        builder.RegisterAssemblyTypes(typeof(UserAccountBaseCommandValidator<,>).GetTypeInfo().Assembly)
               .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
               .AsImplementedInterfaces();

        // Registering generic pipeline behaviors
        // The order of pipelines is crucial as it dictates the sequence in which they are executed
        builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        builder.RegisterGeneric(typeof(TransactionBehavior<,>)).As(typeof(IPipelineBehavior<,>));
    }
}