using EventsWebApplication.Application.Errors;
using EventsWebApplication.Application.Errors.Base;
using EventsWebApplication.Application.Helpers;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Domain.Repositories;
using FluentResults;
using MediatR;

namespace EventsWebApplication.Application.UseCases.Users.Commands.LogoutUser;

public class LogoutUserHandler : IRequestHandler<LogoutUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICookieProvider _cookieProvider;

    public LogoutUserHandler(IUnitOfWork unitOfWork, ICookieProvider cookieProvider)
    {
        _unitOfWork = unitOfWork;
        _cookieProvider = cookieProvider;
    }

    public async Task<Result> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = _cookieProvider.GetCookie("refresh_token");
        _cookieProvider.RemoveCookie($"jwt");
        _cookieProvider.RemoveCookie($"refresh_token");
        
        var user = await _unitOfWork.UserRepository.GetByRefreshTokenAsync(refreshToken, cancellationToken);

        if (user is null)
        {
            return new UserNotFoundError();
        }
        
        await _unitOfWork.UserRepository.RemoveExpiredRefreshTokensAsync(user.UserId, cancellationToken);
        await _unitOfWork.UserRepository.RemoveRefreshTokenAsync(user.UserId, refreshToken, cancellationToken);
        
        var success = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (!success)
        {
            return new InternalServerError("User.Logout", "Failed to save data when removing refresh tokens");
        }

        return Result.Ok();
    }
}