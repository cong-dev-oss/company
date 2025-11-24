using BuildingBlocks.Result;
using MediatR;
using IdentityService.Contracts.DTOs.Requests;

namespace IdentityService.Application.Commands.Register;

public record RegisterCommand(RegisterRequest Request) : IRequest<Result<Contracts.DTOs.Responses.AuthResponse>>;

