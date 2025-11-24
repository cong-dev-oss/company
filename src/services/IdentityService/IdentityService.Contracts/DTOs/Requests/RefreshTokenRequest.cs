using System.ComponentModel.DataAnnotations;

namespace IdentityService.Contracts.DTOs.Requests;

public record RefreshTokenRequest
{
    [Required]
    public string AccessToken { get; init; } = string.Empty;

    [Required]
    public string RefreshToken { get; init; } = string.Empty;
}

