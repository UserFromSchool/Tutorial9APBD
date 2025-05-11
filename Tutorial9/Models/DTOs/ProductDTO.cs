namespace Tutorial9.Models.DTOs;

// Contains basic product information.
public class ProductDTO
{
    public required int IdProduct { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
}