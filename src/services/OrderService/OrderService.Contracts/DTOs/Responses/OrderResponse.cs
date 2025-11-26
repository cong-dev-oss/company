namespace OrderService.Contracts.DTOs.Responses;

public record OrderResponse(
    Guid Id,
    Guid CustomerId,
    AddressResponse ShippingAddress,
    string Status,
    decimal TotalAmount,
    string Currency,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<OrderItemResponse> Items
);

public record AddressResponse(
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country
);

public record OrderItemResponse(
    Guid ProductId,
    string ProductName,
    decimal Price,
    string Currency,
    int Quantity
);







