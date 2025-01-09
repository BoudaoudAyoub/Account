using Account.API.Application.Commands.UserAccountCommands;
using Account.API.Application.Models;
using AutoMapper;
namespace Account.API.Infrastructure.Modules.AutoMapper;

public class ApplicationUserProfile : Profile
{
    public ApplicationUserProfile()
    {
        CreateMap<UserAccountCreateRequest, ApplicationUserCreateCommand>();
        CreateMap<UserAcountUpdateRequest, UserAccountUpdateCommand>();
    }
}