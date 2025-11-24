using BuildingBlocks.Result;
using IdentityService.Application.Commands.Login;
using IdentityService.Application.Commands.RefreshToken;
using IdentityService.Application.Commands.Register;
using IdentityService.Contracts.DTOs.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers;

/// <summary>
/// Authentication and Authorization endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Authentication")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(Contracts.DTOs.Responses.AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand(request);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return result.Error.Code switch
            {
                "CONFLICT" => Conflict(new { error = result.Error.Message }),
                "VALIDATION_ERROR" => BadRequest(new { error = result.Error.Message }),
                _ => BadRequest(new { error = result.Error.Message })
            };
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Authenticate user with email and password
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication response containing access token, refresh token, and user information</returns>
    /// <response code="200">Login successful. Returns authentication tokens.</response>
    /// <response code="401">Invalid credentials or account is locked.</response>
    /// <response code="403">Account is inactive or forbidden.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(Contracts.DTOs.Responses.AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return result.Error.Code switch
            {
                "UNAUTHORIZED" => Unauthorized(new { error = result.Error.Message }),
                "FORBIDDEN" => Forbid(),
                "VALIDATION_ERROR" => BadRequest(new { error = result.Error.Message }),
                _ => Unauthorized(new { error = "Invalid credentials" })
            };
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get current authenticated user information
    /// </summary>
    /// <returns>Current user information including ID, email, name, and roles</returns>
    /// <response code="200">Returns current user information.</response>
    /// <response code="401">Unauthorized. Valid JWT token required.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(Contracts.DTOs.Responses.UserInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        var userInfo = new Contracts.DTOs.Responses.UserInfo
        {
            Id = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString()),
            Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty,
            FirstName = User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value ?? string.Empty,
            LastName = User.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value ?? string.Empty,
            Roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList()
        };

        return Ok(userInfo);
    }

    /// <summary>
    /// Refresh access token using a valid refresh token
    /// </summary>
    /// <param name="request">Expired access token and refresh token</param>
    /// <returns>New authentication tokens</returns>
    /// <response code="200">Tokens refreshed successfully. Returns new access and refresh tokens.</response>
    /// <response code="401">Invalid or expired tokens.</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(Contracts.DTOs.Responses.AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var command = new RefreshTokenCommand(request);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return Unauthorized(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }
}
