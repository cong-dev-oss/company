namespace OrderService.Contracts.DTOs.Requests;

public record CreateOrderRequest(
    Guid CustomerId,
    CreateOrderAddressRequest ShippingAddress,
    List<CreateOrderItemRequest> Items
);

public record CreateOrderAddressRequest(
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country
);

public record CreateOrderItemRequest(
    Guid ProductId,
    string ProductName,
    decimal Price,
    string? Currency,
    int Quantity
);






