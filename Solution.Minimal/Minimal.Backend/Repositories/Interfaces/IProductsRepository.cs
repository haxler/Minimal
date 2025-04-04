using Minimal.Shared.DTOs;
using Minimal.Shared.Entities;
using Minimal.Shared.Responses;

namespace Minimal.Backend.Repositories.Interfaces;

public interface IProductsRepository
{
    Task<ActionResponse<IEnumerable<Product>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);
}
