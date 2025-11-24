using BuildingBlocks.Guards;
using BuildingBlocks.Result;
using MediatR;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Domain.ValueObjects;

namespace OrderService.Application.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(request, nameof(request));
        Guard.AgainstNull(request.ShippingAddress, nameof(request.ShippingAddress));
        Guard.AgainstNullOrEmpty(request.Items, nameof(request.Items));

        var order = new Order(
            Guid.NewGuid(),
            request.CustomerId,
            request.ShippingAddress,
            DateTime.UtcNow
        );

        foreach (var item in request.Items)
        {
            var price = new Money(item.Price, item.Currency ?? "USD");
            order.AddItem(
                item.ProductId,
                item.ProductName,
                price,
                item.Quantity
            );
        }

        await _orderRepository.AddAsync(order, cancellationToken);

        return Result.Success(order.Id);
    }
}

