using Microsoft.Data.SqlClient;

namespace Tutorial9.Repositories;

public class SqlWarehouseRepository(string connectionString) : IWarehouseRepository
{

    private readonly string _connectionString = connectionString;

    public async Task<bool> Exists(int idWarehouse)
    {
        bool foundId = false;
        using (var connection = new SqlConnection(_connectionString))
        using (var command = new SqlCommand("SELECT * FROM Warehouse WHERE idWarehouse = @idWarehouse", connection))
        {
            await connection.OpenAsync();
            command.Parameters.AddWithValue("@idWarehouse", idWarehouse);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    foundId = true;
                    break;
                }
            }
        }

        return foundId;
    }
    
}