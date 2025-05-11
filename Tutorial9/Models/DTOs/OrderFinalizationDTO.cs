namespace Tutorial9.Models.DTOs;

// Used for the final link between the order, product and the warehouse.
public class OrderFinalizationDTO
{
    public int? Id { get; set; }
    public required int IdProduct { get; set; }
    public required int IdWarehouse { get; set; }
    public required int IdOrder { get; set; }
    public required int Amount { get; set; }
    public required decimal Price { get; set; }
    public required DateTime CreatedAt { get; set; }
}