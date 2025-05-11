using Tutorial9.Models.DTOs;

namespace Tutorial9.Repositories;

public interface IOrderRepository
{

    // Returns orders than matches the given product id and their amount.
    public Task<List<OrderDTO>> FindByProductAndAmount(int idProduct, int amount);
    
    // Checks whether given order was already completed (completed means already added to the table linking warehouse and the product).
    public Task<bool> IsCompletedOrder(int idOrder);
    
    // Update the state of the order to the completed. Returns the updated order.
    public Task<OrderDTO> CompleteOrder(int idOrder);
    
    // Finalize the order, by inserting the link between order, product and warehouse. Returns already finalized orderFinalization object with the
    // unique id pointing to this finalization in the system (Warehouse_Product)
    public Task<OrderFinalizationDTO> FinalizeOrder(OrderFinalizationDTO orderFinalization);
    
    // Using a product request automatically handles all the nuances of adding a new product to warehouse
    // using a predefined script and returns results. Result if the id of the new link created.
    // Assumes the procedure has been added to the database.
    public Task<int> AddProductToWarehouseProcedural(ProductRequestDTO request);

}