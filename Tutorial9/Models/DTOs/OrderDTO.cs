namespace Tutorial9.Models.DTOs;

// Contains order information
public class OrderDTO
{
    public required int IdOrder { get; set; }
    public required int IdProduct { get; set; }
    public required int Amount { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? FulfilledAt { get; set; }
}