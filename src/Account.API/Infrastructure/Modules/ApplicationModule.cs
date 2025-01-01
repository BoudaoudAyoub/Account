using Autofac;
using MediatR;
using Account.Infrastructure;
using Account.Domain.Seedwork;
using Account.API.Application.Behaviors;
namespace Account.API.Infrastructure.Modules;

public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Register generic interface 'IRepository' and it's implemented service 'Repository'
        builder.RegisterGeneric(typeof(Repository<,>)).As(typeof(IRepository<,>));

        // Registering generic pipeline behaviors
        // The order of pipelines is crucial as it dictates the sequence in which they are executed
        builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        builder.RegisterGeneric(typeof(TransactionBehavior<,>)).As(typeof(IPipelineBehavior<,>));
    }
}