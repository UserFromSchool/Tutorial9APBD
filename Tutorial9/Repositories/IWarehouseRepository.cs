namespace Tutorial9.Repositories;


// Responsible for retrieving the information about the warehouse
public interface IWarehouseRepository
{
    
    // Checks if given warehous exists.
    public Task<bool> Exists(int idWarehouse);
    
}