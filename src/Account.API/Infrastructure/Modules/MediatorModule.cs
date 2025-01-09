using Account.API.Application.Queries.UserAccounts;
using Autofac;
using FluentValidation;
using Account.Infrastructure;
using Account.Domain.Seedwork;
using Account.Infrastructure.Repositories;
namespace Account.API.Infrastructure.Modules;

public class MediatorModule() : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Register generic interface 'IRepository' and it's implemented service 'Repository'
        builder.RegisterGeneric(typeof(Repository<,>)).As(typeof(IRepository<,>));

        // Register all assembly types their names ends with 'Repository'
        builder.RegisterAssemblyTypes(typeof(ApplicationUserRepository).Assembly)
               .Where(t => t.Name.EndsWith("Repository"))
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();

        // Register all assembly types their names ends with 'Query'
        builder.RegisterAssemblyTypes(typeof(UserAccountQuery).Assembly)
               .Where(t => t.Name.EndsWith("Query"))
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();
    }
}