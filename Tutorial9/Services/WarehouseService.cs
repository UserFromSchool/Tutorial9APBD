using System.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Tutorial9.Models.DTOs;
using Tutorial9.Repositories;

namespace Tutorial9.Services;

public class WarehouseService(IWarehouseRepository warehouseRepository, IProductRepository productRepository, IOrderRepository orderRepository) : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository = warehouseRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<ProductResponseDTO> AddProductToTheWarehouse(ProductRequestDTO request)
    {
        // Check whether the warehouse exists
        if (!await _warehouseRepository.Exists(request.IdWarehouse))
        {
            return new ProductResponseDTO
            {
                Message=$"Warehouse with id {request.IdWarehouse} does not exist."
            };
        }
        
        // Check whether the product exists
        var product = await _productRepository.Find(request.IdProduct);
        if (product is null)
        {
            return new ProductResponseDTO
            {
                Message=$"Product with id {request.IdProduct} does not exist."
            };
        }
        
        // Check if the value passed is correct.
        if (request.Amount <= 0)
        {
            return new ProductResponseDTO
            {
                Message = $"Product with amount {request.Amount} does not exist."
            };
        }
        
        // Get the orders that match base criteria
        var orders = await _orderRepository.FindByProductAndAmount(request.IdProduct, request.Amount);
        if (orders.Count == 0)
        {
            return new ProductResponseDTO
            {
                Message = $"Couldn't find any orders that would be ordering such product to the given warehouse."
            };
        }
        
        // Validate the orders' date
        orders = orders.FindAll(order => order.CreatedAt < request.CreatedAt);
        if (orders.Count > 1)
        {
            return new ProductResponseDTO
            {
                Message = $"More than one order matching given criteria found."
            };
        }
        if (orders.Count == 0)
        {
            return new ProductResponseDTO
            {
                Message = $"Couldn't find any orders matching the datetime criteria of this request."
            };
        }
        
        // Checks whether given order was already completed
        var order = orders.First();
        if (await _orderRepository.IsCompletedOrder(order.IdOrder))
        {
            return new ProductResponseDTO
            {
                Message = $"Order with id {order.IdOrder}, which was matching the specified details is already completed."
            };
        }
        
        // Try to complete the order
        var completedOrder = await _orderRepository.CompleteOrder(order.IdOrder);

        // Insert the new link between order, product and warehouse to finalize the order
        var finalization = new OrderFinalizationDTO
        {
            IdProduct = request.IdProduct,
            IdWarehouse = request.IdWarehouse,
            IdOrder = completedOrder.IdOrder,
            CreatedAt = completedOrder.CreatedAt,
            Amount = completedOrder.Amount,
            Price = product.Price * completedOrder.Amount
        };
        finalization = await _orderRepository.FinalizeOrder(finalization);
        
        // Once finalized, return the success response
        return new ProductResponseDTO
        {
            Id = finalization.Id,
            Message = "Successfully completed the order for the specified details."
        };
    }


    public async Task<ProductResponseDTO> AddProductToTheWarehouseProcedural(ProductRequestDTO request)
    {
        try
        {
            var newId = await _orderRepository.AddProductToWarehouseProcedural(request);
            return new ProductResponseDTO
            {
                Id = newId, Message = "Successfully completed the order for the specified details."
            };
        }
        catch (SqlException e)
        {
            return new ProductResponseDTO
            {
                Id = -1, Message = e.Message
            };
        }
    }
    
}