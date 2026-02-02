using AutoMapper;
using MediatR;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs;
using InventorySystem.Domain.Interfaces;

namespace InventorySystem.Application.Features.Auth.Commands;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;

    public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, IJwtTokenGenerator jwtTokenGenerator, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
        _mapper = mapper;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _jwtTokenGenerator.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
            return Result<AuthResponse>.Failure("Token inválido.");

        var userId = principal.FindFirst("uid")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Result<AuthResponse>.Failure("Token inválido.");

        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return Result<AuthResponse>.Failure("Token de actualización inválido o expirado.");

        var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        var refreshTokenExpiry = _jwtTokenGenerator.GetRefreshTokenExpiry();

        await _unitOfWork.Users.UpdateRefreshTokenAsync(user.Id, newRefreshToken, refreshTokenExpiry, cancellationToken);

        return Result<AuthResponse>.Success(new AuthResponse(
            newAccessToken,
            newRefreshToken,
            _mapper.Map<UserDto>(user),
            DateTime.UtcNow.AddHours(1)
        ));
    }
}
