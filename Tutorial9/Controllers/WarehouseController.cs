using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Tutorial9.Models.DTOs;
using Tutorial9.Services;

namespace Tutorial9.Controllers;

[ApiController]
[Route("[controller]")]
public class WarehouseController(IWarehouseService warehouseService) : Controller
{

    private readonly IWarehouseService _warehouseService = warehouseService;

    // Api endpoint for adding new product (which was already ordered) to the warehouse. Done without SQL procedure.
    [HttpPost("products/add")]
    public async Task<IActionResult> AddProduct(ProductRequestDTO request)
    {
        try
        {
            var response = await _warehouseService.AddProductToTheWarehouse(request);
            if (response.Id is null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.Message);
            return StatusCode(500);
        }
    }
    
    // Api endpoint for adding new product (which was already ordered) to the warehouse. Done with SQL procedure.
    [HttpPost("procedural/products/add")]
    public async Task<IActionResult> AddProceduralProduct(ProductRequestDTO request)
    {
        var response = await _warehouseService.AddProductToTheWarehouseProcedural(request);
        if (response.Id == -1)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
    
}