using Autofac;
using Account.Infrastructure;
using Account.Domain.Seedwork;
namespace Account.API.Infrastructure.Modules;

public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Register generic interface 'IRepository' and it's implemented service 'Repository'
        builder.RegisterGeneric(typeof(Repository<,>)).As(typeof(IRepository<,>));
    }
}