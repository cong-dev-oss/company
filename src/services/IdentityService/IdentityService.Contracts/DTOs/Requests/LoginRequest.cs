using System.ComponentModel.DataAnnotations;

namespace IdentityService.Contracts.DTOs.Requests;

public record LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}

