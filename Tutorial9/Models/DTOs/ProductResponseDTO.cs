namespace Tutorial9.Models.DTOs;

// Contains information returned from the API when adding product to the warehouse.
public class ProductResponseDTO
{
    public int? Id { get; set; }
    public required string Message { get; set; }
}