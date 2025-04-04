using Microsoft.AspNetCore.Mvc;
using Minimal.Backend.UnitsOfWork.Interfaces;
using Minimal.Shared.DTOs;
using Minimal.Shared.Entities;

namespace Minimal.Backend.Controllers;

[Route("api/[controller]")]
public class ProductsController : GenericController<Product>
{
    private readonly IProductsUnitOfWork _ProductsUnitOfWork;

    public ProductsController(IGenericUnitOfWork<Product> unit, IProductsUnitOfWork ProductsUnitOfWork) : base(unit)
    {
        _ProductsUnitOfWork = ProductsUnitOfWork;
    }

    [HttpGet]
    public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _ProductsUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("totalPages")]
    public override async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _ProductsUnitOfWork.GetTotalPagesAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }
}