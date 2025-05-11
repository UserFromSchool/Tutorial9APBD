using Microsoft.Data.SqlClient;
using Tutorial9.Models.DTOs;

namespace Tutorial9.Repositories;

public class SqlProductRepository(string connectionString) : IProductRepository
{

    private readonly string _connectionString = connectionString;

    public async Task<ProductDTO?> Find(int idProduct)
    {
        ProductDTO? product = null;
        using (var connection = new SqlConnection(_connectionString))
        using (var command = new SqlCommand("SELECT * FROM Product WHERE idProduct = @idProduct", connection))
        {
            await connection.OpenAsync();
            command.Parameters.AddWithValue("@idProduct", idProduct);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    product = new ProductDTO
                    {
                        IdProduct = reader.GetInt32(reader.GetOrdinal("idProduct")),
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        Description = reader.GetString(reader.GetOrdinal("description")),
                        Price = reader.GetDecimal(reader.GetOrdinal("price"))
                    };
                }
            }
        }
        
        return product;
    }
    
}