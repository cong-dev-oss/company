namespace OrderService.Domain.DomainEvents;

/// <summary>
/// Domain event raised when an order is created
/// </summary>
public record OrderCreatedEvent(Guid OrderId, Guid CustomerId, DateTime CreatedAt) : IDomainEvent;

public interface IDomainEvent
{
}







