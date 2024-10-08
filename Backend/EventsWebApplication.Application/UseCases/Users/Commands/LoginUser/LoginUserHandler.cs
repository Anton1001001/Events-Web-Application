using AutoMapper;
using EventsWebApplication.Application.Errors.Base;
using EventsWebApplication.Application.Helpers;
using EventsWebApplication.Domain.Repositories;
using FluentResults;
using MediatR;

namespace EventsWebApplication.Application.UseCases.Users.Commands.LoginUser;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, Result<UserResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IJwtProvider _jwtProvider;
    private readonly ICookieProvider _cookieProvider;

    public LoginUserHandler(IUnitOfWork unitOfWork, IMapper mapper, IJwtProvider jwtProvider, ICookieProvider cookieProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _jwtProvider = jwtProvider;
        _cookieProvider = cookieProvider;
    }

    public async Task<Result<UserResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email, cancellationToken);
        
        if (user is null)
        {
            return new InvalidCredentialsError();
        }
        
        bool passwordValid = _jwtProvider.VerifyPassword(request.Password, user.PasswordHash);
        
        if (!passwordValid)
        {
            return new InvalidCredentialsError();
        }
        
        var jwtToken = _jwtProvider.GenerateJwtToken(user);
        var refreshTokenEntity = _jwtProvider.GenerateRefreshToken();
        
        _cookieProvider.SetCookie("jwt", jwtToken);
        _cookieProvider.SetCookie("refresh_token", refreshTokenEntity.Token);
        
        await _unitOfWork.UserRepository.RemoveExpiredRefreshTokensAsync(user.UserId, cancellationToken);
        await _unitOfWork.UserRepository.AddRefreshTokenAsync(user.UserId, refreshTokenEntity, cancellationToken);
        
        var success = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (!success)
        {
            return new InternalServerError("User.Login", message: "Failed to save data");
        }

        return _mapper.Map<UserResponse>(user);

    }
}