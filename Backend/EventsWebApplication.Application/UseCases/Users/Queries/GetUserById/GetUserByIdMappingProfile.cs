using AutoMapper;
using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Application.UseCases.Users.Queries.GetUserById;

public class GetUserByIdMappingProfile : Profile
{
    public GetUserByIdMappingProfile()
    {
        CreateMap<User, UserResponse>();
    }
    
}