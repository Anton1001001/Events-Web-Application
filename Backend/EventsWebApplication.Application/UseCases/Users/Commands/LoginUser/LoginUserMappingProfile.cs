using AutoMapper;
using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Application.UseCases.Users.Commands.LoginUser;

public class LoginUserMappingProfile : Profile
{
    public LoginUserMappingProfile()
    {
        CreateMap<User, UserResponse>();
    }
}