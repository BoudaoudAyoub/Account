using System.Reflection;
using Autofac;
using MediatR;

namespace Account.API.Infrastructure.Modules;

public class MediatorModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Register IMediator interface
        builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();
    }
}