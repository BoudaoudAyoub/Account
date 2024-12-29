using Autofac;
using AutoMapper;

namespace Account.API.Infrastructure.Modules;
public class AutoMapperLoad : Autofac.Module
{
    public void AutoMapperLoads(ContainerBuilder containerBuilder)
    {
        containerBuilder.Register(context => MapperConfiguration())
                        .AsSelf()
                        .SingleInstance();

        containerBuilder.Register(context =>
        {
            context.Resolve<IComponentContext>();
            return context.Resolve<MapperConfiguration>().CreateMapper(context.Resolve);

        }).As<IMapper>().InstancePerLifetimeScope();
    }

    private static MapperConfiguration MapperConfiguration() => new(mapper =>
    {
        //TODO: adding mapper profiles' configs
    });
}