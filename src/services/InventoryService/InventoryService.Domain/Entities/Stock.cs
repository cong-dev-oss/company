namespace InventoryService.Domain.Entities;

public class Stock
{
    private Stock() { } // EF Core constructor

    public Stock(Guid id, Guid productId, int quantity, string location)
    {
        Id = id;
        ProductId = productId;
        Quantity = quantity;
        Location = location;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public string Location { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public void AdjustQuantity(int adjustment)
    {
        var newQuantity = Quantity + adjustment;
        if (newQuantity < 0)
            throw new InvalidOperationException("Insufficient stock");

        Quantity = newQuantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetQuantity(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(quantity));

        Quantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }
}


