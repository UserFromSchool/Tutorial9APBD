namespace Tutorial9.Models.DTOs;

// Request for inserting the product to the warehouse.
public class ProductRequestDTO
{
    public required int IdProduct { get; set; }
    public required int IdWarehouse { get; set; }
    public required int Amount { get; set; }
    public required DateTime CreatedAt { get; set; }
}