using AutoMapper;
using EventsWebApplication.Application.Errors.Base;
using EventsWebApplication.Application.Helpers;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Domain.Repositories;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EventsWebApplication.Application.UseCases.Users.Commands.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<UserResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserHandler(IUnitOfWork unitOfWork, IMapper mapper, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UserResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email, cancellationToken);
        
        if (existingUser is not null)
        {
            return new UserEmailConflictError(message: "User with such email already exists");
        }
        
        string passwordHash = _passwordHasher.HashPassword(request.Password);
        
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
        };
        
        var addedUser = await _unitOfWork.UserRepository.AddAsync(user, cancellationToken);
        
        var success = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (!success)
        {
            return new InternalServerError("User.Register", "Failed to save data when add a new user");
        }
        
        
        return _mapper.Map<UserResponse>(addedUser);
        
    }
}