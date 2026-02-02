using AutoMapper;
using MediatR;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs;
using InventorySystem.Domain.Interfaces;

namespace InventorySystem.Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;

    public LoginCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, 
        IJwtTokenGenerator jwtTokenGenerator, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _mapper = mapper;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email.ToLowerInvariant(), cancellationToken);

        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return Result<AuthResponse>.Failure("Credenciales inválidas.");

        if (!user.IsActive)
            return Result<AuthResponse>.Failure("La cuenta está desactivada.");

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        var refreshTokenExpiry = _jwtTokenGenerator.GetRefreshTokenExpiry();

        await _unitOfWork.Users.UpdateRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiry, cancellationToken);

        return Result<AuthResponse>.Success(new AuthResponse(
            accessToken,
            refreshToken,
            _mapper.Map<UserDto>(user),
            DateTime.UtcNow.AddHours(1)
        ));
    }
}
