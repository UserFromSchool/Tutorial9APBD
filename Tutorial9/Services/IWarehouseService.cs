using Tutorial9.Models.DTOs;

namespace Tutorial9.Services;

public interface IWarehouseService
{

    public Task<ProductResponseDTO> AddProductToTheWarehouse(ProductRequestDTO request);
    
    public Task<ProductResponseDTO> AddProductToTheWarehouseProcedural(ProductRequestDTO request);

}