using Tutorial9.Models.DTOs;

namespace Tutorial9.Repositories;


// Responsible for retrieving the information about the product
public interface IProductRepository
{

    // Finds a given product and returns it if it exists.
    public Task<ProductDTO?> Find(int idProduct);

}