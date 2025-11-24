using BuildingBlocks.Result;
using MediatR;
using IdentityService.Contracts.DTOs.Requests;
using IdentityService.Contracts.DTOs.Responses;

namespace IdentityService.Application.Commands.RefreshToken;

public record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<Result<AuthResponse>>;

