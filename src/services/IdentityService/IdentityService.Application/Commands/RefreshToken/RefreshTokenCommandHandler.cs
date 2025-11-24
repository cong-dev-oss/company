using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using BuildingBlocks.Result;
using IdentityService.Contracts.DTOs.Responses;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Security.Jwt;

namespace IdentityService.Application.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        UserManager<User> userManager,
        IJwtTokenService jwtTokenService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Get principal from expired token
        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(request.Request.AccessToken);
        if (principal == null)
        {
            return Result.Failure<AuthResponse>(Error.Unauthorized());
        }

        // Get user ID from claims
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Result.Failure<AuthResponse>(Error.Unauthorized());
        }

        // Get JWT ID from claims
        var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        if (string.IsNullOrEmpty(jti))
        {
            return Result.Failure<AuthResponse>(Error.Unauthorized());
        }

        // Find refresh token in database
        var storedRefreshToken = await _refreshTokenRepository.GetByTokenAsync(request.Request.RefreshToken, userId, cancellationToken);

        if (storedRefreshToken == null)
        {
            return Result.Failure<AuthResponse>(Error.Unauthorized());
        }

        // Validate refresh token
        if (storedRefreshToken.IsUsed || storedRefreshToken.IsRevoked)
        {
            return Result.Failure<AuthResponse>(Error.Unauthorized());
        }

        if (storedRefreshToken.ExpiresAt < DateTime.UtcNow)
        {
            return Result.Failure<AuthResponse>(Error.Unauthorized());
        }

        if (storedRefreshToken.JwtId != jti)
        {
            return Result.Failure<AuthResponse>(Error.Unauthorized());
        }

        // Get user
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || !user.IsActive)
        {
            return Result.Failure<AuthResponse>(Error.Unauthorized());
        }

        // Mark refresh token as used
        storedRefreshToken.IsUsed = true;
        await _refreshTokenRepository.UpdateAsync(storedRefreshToken, cancellationToken);

        // Generate new tokens
        var claims = await GetUserClaimsAsync(user);
        var newAccessToken = _jwtTokenService.GenerateAccessToken(claims);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
        var newJti = claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

        // Save new refresh token
        var newStoredRefreshToken = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = newRefreshToken,
            JwtId = newJti,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // Refresh token expires in 7 days
            IsUsed = false,
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(newStoredRefreshToken, cancellationToken);

        var userRoles = await _userManager.GetRolesAsync(user);

        var authResponse = new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
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

