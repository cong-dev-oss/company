using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Entities;

public class OrderItem
{
    private OrderItem() { } // EF Core constructor

    public OrderItem(Guid productId, string productName, Money price, int quantity)
    {
        ProductId = productId;
        ProductName = productName;
        Price = price;
        Quantity = quantity;
    }

    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public Money Price { get; private set; } = null!;
    public int Quantity { get; private set; }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));

        Quantity = newQuantity;
    }
}

