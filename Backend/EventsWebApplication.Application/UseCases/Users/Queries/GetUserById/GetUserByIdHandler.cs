using AutoMapper;
using EventsWebApplication.Application.Errors;
using EventsWebApplication.Domain.Repositories;
using FluentResults;
using MediatR;

namespace EventsWebApplication.Application.UseCases.Users.Queries.GetUserById;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, Result<UserResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return new UserNotFoundError(message: $"User with id: {request.UserId} was not found");
        }
        return _mapper.Map<UserResponse>(user);
    }
}