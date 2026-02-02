using AutoMapper;
using MediatR;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs;
using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Interfaces;

namespace InventorySystem.Application.Features.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;

    public RegisterCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _mapper = mapper;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Users.ExistsByEmailAsync(request.Email, cancellationToken))
            return Result<AuthResponse>.Failure("El correo electrónico ya está registrado.");

        var user = new User
        {
            Email = request.Email.ToLowerInvariant(),
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role
        };

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        var refreshTokenExpiry = _jwtTokenGenerator.GetRefreshTokenExpiry();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = refreshTokenExpiry;

        await _unitOfWork.Users.AddAsync(user, cancellationToken);

        return Result<AuthResponse>.Success(new AuthResponse(
            accessToken,
            refreshToken,
            _mapper.Map<UserDto>(user),
            DateTime.UtcNow.AddHours(1)
        ));
    }
}
