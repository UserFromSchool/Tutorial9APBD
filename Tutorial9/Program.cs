using Tutorial9.Repositories;
using Tutorial9.Services;

namespace Tutorial9;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (connectionString is null)
        {
            throw new ArgumentException("Can't find the connection string in the application.json settings.");
        }

        builder.Services.AddSingleton(connectionString);
        builder.Services.AddScoped<IOrderRepository, SqlOrderRepository>();
        builder.Services.AddScoped<IProductRepository, SqlProductRepository>();
        builder.Services.AddScoped<IWarehouseRepository, SqlWarehouseRepository>();
        builder.Services.AddScoped<IWarehouseService, WarehouseService>();
        builder.Services.AddControllers();
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}