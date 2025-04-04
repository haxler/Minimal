using Minimal.Shared.DTOs;
using Minimal.Shared.Entities;
using Minimal.Shared.Responses;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Minimal.Backend.UnitsOfWork.Interfaces;

public interface IProductsUnitOfWork
{
    Task<ActionResponse<IEnumerable<Product>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);
}
