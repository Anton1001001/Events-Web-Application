using AutoMapper;
using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Application.UseCases.Users.Commands.RegisterUser;

public class RegisterUserMappingProfile : Profile
{
    public RegisterUserMappingProfile()
    {
        CreateMap<User, UserResponse>();
    }
}