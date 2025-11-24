using BuildingBlocks.Result;
using MediatR;
using OrderService.Domain.ValueObjects;

namespace OrderService.Application.Commands.CreateOrder;

public record CreateOrderCommand(
    Guid CustomerId,
    Address ShippingAddress,
    List<OrderItemDto> Items
) : IRequest<Result<Guid>>;

public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    decimal Price,
    string Currency,
    int Quantity
);

