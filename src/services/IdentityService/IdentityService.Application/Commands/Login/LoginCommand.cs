using BuildingBlocks.Result;
using MediatR;
using IdentityService.Contracts.DTOs.Requests;
using IdentityService.Contracts.DTOs.Responses;

namespace IdentityService.Application.Commands.Login;

public record LoginCommand(LoginRequest Request) : IRequest<Result<AuthResponse>>;

