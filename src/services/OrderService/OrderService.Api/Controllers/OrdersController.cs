using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Contracts.DTOs.Requests;
using OrderService.Contracts.DTOs.Responses;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Domain.ValueObjects;
using OrderService.Security.Authorization.Policies;

namespace OrderService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IOrderRepository _orderRepository;

    public OrdersController(IMediator mediator, IOrderRepository orderRepository)
    {
        _mediator = mediator;
        _orderRepository = orderRepository;
    }

    /// <summary>
    /// Create a new order
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        // Get current user ID from JWT token
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? User.FindFirstValue("sub") 
            ?? throw new UnauthorizedAccessException("User ID not found in token");

        if (!Guid.TryParse(userId, out var customerId))
        {
            return BadRequest("Invalid user ID");
        }

        // Map request to command
        var command = new CreateOrderCommand(
            customerId,
            new Address(
                request.ShippingAddress.Street,
                request.ShippingAddress.City,
                request.ShippingAddress.State,
                request.ShippingAddress.ZipCode,
                request.ShippingAddress.Country
            ),
            request.Items.Select(item => new OrderItemDto(
                item.ProductId,
                item.ProductName,
                item.Price,
                item.Currency ?? "USD",
                item.Quantity
            )).ToList()
        );

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return CreatedAtAction(nameof(GetOrder), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetOrder(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);

        if (order == null)
        {
            return NotFound();
        }

        // Check authorization: User must own the order
        var authorizationResult = await HttpContext.RequestServices
            .GetRequiredService<IAuthorizationService>()
            .AuthorizeAsync(User, order.CustomerId, OrderOwnerPolicy.Name);

        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        var response = MapToResponse(order);
        return Ok(response);
    }

    /// <summary>
    /// Get orders for current user
    /// </summary>
    [HttpGet("my-orders")]
    [ProducesResponseType(typeof(List<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyOrders(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? User.FindFirstValue("sub") 
            ?? throw new UnauthorizedAccessException("User ID not found in token");

        if (!Guid.TryParse(userId, out var customerId))
        {
            return BadRequest("Invalid user ID");
        }

        var orders = await _orderRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        var response = orders.Select(MapToResponse).ToList();

        return Ok(response);
    }

    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse(
            order.Id,
            order.CustomerId,
            new AddressResponse(
                order.ShippingAddress.Street,
                order.ShippingAddress.City,
                order.ShippingAddress.State,
                order.ShippingAddress.ZipCode,
                order.ShippingAddress.Country
            ),
            order.Status.ToString(),
            order.TotalAmount.Amount,
            order.TotalAmount.Currency,
            order.CreatedAt,
            order.UpdatedAt,
            order.Items.Select(item => new OrderItemResponse(
                item.ProductId,
                item.ProductName,
                item.Price.Amount,
                item.Price.Currency,
                item.Quantity
            )).ToList()
        );
    }
}

