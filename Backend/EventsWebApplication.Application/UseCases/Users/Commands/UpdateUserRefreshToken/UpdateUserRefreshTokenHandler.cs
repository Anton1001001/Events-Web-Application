using EventsWebApplication.Application.Errors;
using EventsWebApplication.Application.Errors.Base;
using EventsWebApplication.Application.Helpers;
using EventsWebApplication.Domain.Repositories;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace EventsWebApplication.Application.UseCases.Users.Commands.UpdateUserRefreshToken;

public class UpdateUserRefreshTokenHandler : IRequestHandler<UpdateUserRefreshTokenCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICookieProvider _cookieProvider;
    private readonly IJwtProvider _jwtProvider;

    public UpdateUserRefreshTokenHandler(IUnitOfWork unitOfWork, ICookieProvider cookieProvider, IJwtProvider jwtProvider)
    {
        _unitOfWork = unitOfWork;
        _cookieProvider = cookieProvider;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result> Handle(UpdateUserRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = _cookieProvider.GetCookie("refresh_token");  
        
        var user = await _unitOfWork.UserRepository.GetByRefreshTokenAsync(refreshToken, cancellationToken);

        if (user is null)
        {
            return new UserNotFoundError();
        }
        
        await _unitOfWork.UserRepository.RemoveExpiredRefreshTokensAsync(user.UserId, cancellationToken);
        await _unitOfWork.UserRepository.RemoveRefreshTokenAsync(user.UserId, refreshToken, cancellationToken);
        
        var newJwtToken = _jwtProvider.GenerateJwtToken(user);
        var newRefreshToken = _jwtProvider.GenerateRefreshToken();
        
        _cookieProvider.SetCookie($"jwt", newJwtToken);
        _cookieProvider.SetCookie($"refresh_token", newRefreshToken.Token);
        
        await _unitOfWork.UserRepository.AddRefreshTokenAsync(user.UserId, newRefreshToken, cancellationToken);
        
        var success = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (!success)
        {
            return new InternalServerError("User.UpdateRefreshToken", "Failed to save data when update refresh token");
        }
        
        return Result.Ok();
    }
    
}