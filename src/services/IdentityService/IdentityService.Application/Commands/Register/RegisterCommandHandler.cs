using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using BuildingBlocks.Result;
using IdentityService.Contracts.DTOs.Responses;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Security.Jwt;

namespace IdentityService.Application.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IJwtTokenService _jwtTokenService;

    public RegisterCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IJwtTokenService jwtTokenService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Request.Email);
        if (existingUser != null)
        {
            return Result.Failure<AuthResponse>(Error.Conflict("User with this email already exists"));
        }

        // Create new user
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.Request.Email,
            Email = request.Request.Email,
            FirstName = request.Request.FirstName,
            LastName = request.Request.LastName,
            EmailConfirmed = true, // In production, should verify email
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure<AuthResponse>(Error.Validation(errors));
        }

        // Assign default role (User)
        var defaultRole = "User";
        if (await _roleManager.RoleExistsAsync(defaultRole))
        {
            await _userManager.AddToRoleAsync(user, defaultRole);
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

