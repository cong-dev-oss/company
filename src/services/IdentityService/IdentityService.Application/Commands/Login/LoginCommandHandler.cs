using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using BuildingBlocks.Result;
using IdentityService.Contracts.DTOs.Responses;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Security.Jwt;

namespace IdentityService.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtTokenService jwtTokenService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Request.Email);
        if (user == null)
        {
            return Result.Failure<AuthResponse>(Error.Unauthorized());
        }

        if (!user.IsActive)
        {
            return Result.Failure<AuthResponse>(Error.Forbidden());
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                return Result.Failure<AuthResponse>(Error.Validation("Account is locked out"));
            }
            return Result.Failure<AuthResponse>(Error.Unauthorized());
        }

        // Generate tokens
        var claims = await GetUserClaimsAsync(user);
        var accessToken = _jwtTokenService.GenerateAccessToken(claims);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        var jti = claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

        // Save refresh token to database
        var storedRefreshToken = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            JwtId = jti,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // Refresh token expires in 7 days
            IsUsed = false,
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(storedRefreshToken, cancellationToken);

        var userRoles = await _userManager.GetRolesAsync(user);

        var authResponse = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = _jwtTokenService.GetTokenExpiration(),
            User = new UserInfo
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = userRoles.ToList()
            }
        };

        return Result.Success(authResponse);
    }

    private async Task<List<Claim>> GetUserClaimsAsync(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }
}

