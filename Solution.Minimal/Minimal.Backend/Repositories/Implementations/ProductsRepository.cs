using Microsoft.EntityFrameworkCore;
using Minimal.Backend.Data;
using Minimal.Backend.Helpers;
using Minimal.Backend.Repositories.Interfaces;
using Minimal.Shared.DTOs;
using Minimal.Shared.Entities;
using Minimal.Shared.Responses;

namespace Minimal.Backend.Repositories.Implementations;

public class ProductsRepository : GenericRepository<Product>, IProductsRepository
{
    private readonly DataContext _context;

    public ProductsRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Product>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Product>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Name)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
    {
        var queryable = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        double count = await queryable.CountAsync();
        int totalPages = (int)Math.Ceiling(count / pagination.RecordsNumber);
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = totalPages
        };
    }
}