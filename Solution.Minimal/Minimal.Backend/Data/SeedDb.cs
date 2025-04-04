using Minimal.Shared.Entities;

namespace Minimal.Backend.Data;


public class SeedDb
{
    private readonly DataContext _context;

    public SeedDb(DataContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync(); // Metodo usado para correr las migraciones pendientes de la BD
        await CheckProductsAsync();
    }

    private async Task CheckProductsAsync()
    {
        // Cuando es primera vez llenamos la tabla inicial
        if (!_context.Products.Any())
        {
            _context.Products.Add(new Product { Name = "TELEVISOR", Price = 500000, Quantity = 5 });
            _context.Products.Add(new Product { Name = "NEVERA", Price = 800000, Quantity = 10 });
            _context.Products.Add(new Product { Name = "LAVADORA", Price = 800000, Quantity = 10 });
            _context.Products.Add(new Product { Name = "SECADORA", Price = 800000, Quantity = 10 });
            _context.Products.Add(new Product { Name = "ARROCERA", Price = 800000, Quantity = 10 });
            _context.Products.Add(new Product { Name = "CAMA", Price = 800000, Quantity = 10 });
        }

        await _context.SaveChangesAsync();
    }
}