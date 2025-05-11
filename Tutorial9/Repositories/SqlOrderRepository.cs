using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using Tutorial9.Models.DTOs;

namespace Tutorial9.Repositories;

// Uses SQL database as a source for the IOrderRepository interface.
public class SqlOrderRepository(string connectionString) : IOrderRepository
{
    
    private readonly string _connectionString = connectionString;

    public async Task<List<OrderDTO>> FindByProductAndAmount(int idProduct, int amount)
    {
        var orders = new List<OrderDTO>();
        using (var connection = new SqlConnection(_connectionString))
        using (var command = new SqlCommand("SELECT * FROM \"Order\" WHERE IdProduct = @idProduct AND Amount = @amount", connection))
        {
            await connection.OpenAsync();
            command.Parameters.AddWithValue("@idProduct", idProduct);
            command.Parameters.AddWithValue("@amount", amount);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    orders.Add(new OrderDTO
                    {
                        IdOrder = reader.GetInt32(reader.GetOrdinal("IdOrder")),
                        IdProduct = reader.GetInt32(reader.GetOrdinal("IdProduct")),
                        Amount = reader.GetInt32(reader.GetOrdinal("Amount")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                    });
                }
            }
            return orders;
        }
    }

    public async Task<bool> IsCompletedOrder(int idOrder)
    {
        var completed = false;
        using (var connection = new SqlConnection(_connectionString))
        using (var command = new SqlCommand("SELECT * FROM Product_Warehouse WHERE IdOrder=@idOrder", connection))
        {
            await connection.OpenAsync();
            command.Parameters.AddWithValue("@idOrder", idOrder);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    completed = true;
                    break;
                }
            }
        }

        return completed;
    }


    public async Task<OrderDTO> CompleteOrder(int idOrder)
    {
        OrderDTO? newOrder = null;
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            using (var command =
                         new SqlCommand("UPDATE \"Order\" SET FulfilledAt = @fulfilledAt WHERE IdOrder = @idOrder",
                             connection))
            {
                var transaction = await connection.BeginTransactionAsync();
                command.Transaction = transaction as SqlTransaction;

                try
                {
                    command.Parameters.AddWithValue("@idOrder", idOrder);
                    command.Parameters.AddWithValue("@fulfilledAt", DateAndTime.Now);
                    await command.ExecuteNonQueryAsync();
                    await transaction.CommitAsync();
                }
                catch (SqlException)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            using (var command = new SqlCommand("SELECT * FROM \"Order\" WHERE IdOrder=@idOrder", connection))
            {
                command.Parameters.AddWithValue("@idOrder", idOrder);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        newOrder = new OrderDTO
                        {
                            IdOrder = reader.GetInt32(reader.GetOrdinal("IdOrder")),
                            IdProduct = reader.GetInt32(reader.GetOrdinal("IdProduct")),
                            Amount = reader.GetInt32(reader.GetOrdinal("Amount")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                            FulfilledAt = reader.GetDateTime(reader.GetOrdinal("FulfilledAt"))
                        };
                    }
                }
            }
        }
        
        return newOrder!;
    }

    public async Task<OrderFinalizationDTO> FinalizeOrder(OrderFinalizationDTO orderFinalization)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var transaction = await connection.BeginTransactionAsync();

            try
            {
                using (var command = new SqlCommand(
                                 "INSERT INTO Product_Warehouse (IdProduct, IdOrder, IdWarehouse, Amount, Price, CreatedAt) " +
                                 "OUTPUT INSERTED.IdProductWarehouse " +
                                 "VALUES (@idProduct, @idOrder, @idWarehouse, @amount, @price, @createdAt)", connection, transaction as SqlTransaction))
                {

                    command.Parameters.AddWithValue("@idProduct", orderFinalization.IdProduct);
                    command.Parameters.AddWithValue("@idOrder", orderFinalization.IdOrder);
                    command.Parameters.AddWithValue("@idWarehouse", orderFinalization.IdWarehouse);
                    command.Parameters.AddWithValue("@amount", orderFinalization.Amount);
                    command.Parameters.AddWithValue("@price", orderFinalization.Price);
                    command.Parameters.AddWithValue("@createdAt", orderFinalization.CreatedAt);
                    var result = await command.ExecuteScalarAsync();
                    if (result != null && result != DBNull.Value)
                    {
                        orderFinalization.Id = Convert.ToInt32(result);
                    }
                }
                
                await transaction.CommitAsync();
            } catch (SqlException)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
        return orderFinalization;
    }


    public async Task<int> AddProductToWarehouseProcedural(ProductRequestDTO request)
    {
        var newId = -1;
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            using (var command = new SqlCommand("AddProductToWarehouse", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdProduct", request.IdProduct);
                command.Parameters.AddWithValue("@IdWarehouse", request.IdWarehouse);
                command.Parameters.AddWithValue("@Amount", request.Amount);
                command.Parameters.AddWithValue("@CreatedAt", request.CreatedAt);
                var result = await command.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value) {
                    newId = Convert.ToInt32(result);
                }
            }
        }
        
        return newId;
    }
    
}