using OrderService.Domain.DomainEvents;
using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Entities;

/// <summary>
/// Order aggregate root entity
/// </summary>
public class Order
{
    private readonly List<OrderItem> _items = new();

    private Order() { } // EF Core constructor

    public Order(Guid id, Guid customerId, Address shippingAddress, DateTime createdAt)
    {
        Id = id;
        CustomerId = customerId;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public Address ShippingAddress { get; private set; } = null!;
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public Money TotalAmount => new(_items.Sum(item => item.Price.Amount * item.Quantity));

    public void AddItem(Guid productId, string productName, Money price, int quantity)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot modify order that is not in Pending status");

        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            _items.Add(new OrderItem(productId, productName, price, quantity));
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed");

        if (!_items.Any())
            throw new InvalidOperationException("Cannot confirm order without items");

        Status = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;

        // Domain event would be raised here
        // RaiseDomainEvent(new OrderConfirmedEvent(Id));
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Cancelled || Status == OrderStatus.Shipped)
            throw new InvalidOperationException("Cannot cancel order in current status");

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}

