using Microsoft.EntityFrameworkCore;
using Minimal.Backend.Data;
using Minimal.Backend.Helpers;
using Minimal.Backend.Repositories.Interfaces;
using Minimal.Shared.DTOs;
using Minimal.Shared.Responses;

namespace Minimal.Backend.Repositories.Implementations;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly DataContext _context;
    private readonly DbSet<T> _entity;

    public GenericRepository(DataContext context)
    {
        _context = context;
        _entity = context.Set<T>();
    }

    public virtual async Task<ActionResponse<IEnumerable<T>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _entity.AsQueryable();

        return new ActionResponse<IEnumerable<T>>
        {
            WasSuccess = true,
            Result = await queryable
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public virtual async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
    {
        var queryable = _entity.AsQueryable();
        double count = await queryable.CountAsync();
        int totalPages = (int)Math.Ceiling(count / pagination.RecordsNumber);
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = totalPages
        };
    }

    public virtual async Task<ActionResponse<T>> AddAsync(T entity)
    {
        _context.Add(entity);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<T>
            {
                WasSuccess = true,
                Result = entity
            };
        }
        catch (DbUpdateException)
        {
            return DbUpdateExceptionActionResponse();
        }
        catch (Exception exception)
        {
            return ExceptionActionResponse(exception);
        }
    }

    public virtual async Task<ActionResponse<T>> DeleteAsync(int id)
    {
        var row = await _entity.FindAsync(id);
        if (row == null)
        {
            return new ActionResponse<T>
            {
                WasSuccess = false,
                Message = "Registro no encontrado"
            };
        }

        try
        {
            _entity.Remove(row);
            await _context.SaveChangesAsync();
            return new ActionResponse<T>
            {
                WasSuccess = true,
            };
        }
        catch
        {
            return new ActionResponse<T>
            {
                WasSuccess = false,
                Message = "No se puede borrar, porque tiene registros relacionados"
            };
        }
    }

    public virtual async Task<ActionResponse<T>> GetAsync(int id)
    {
        var row = await _entity.FindAsync(id);
        if (row != null)
        {
            return new ActionResponse<T>
            {
                WasSuccess = true,
                Result = row
            };
        }
        return new ActionResponse<T>
        {
            WasSuccess = false,
            Message = "Registro no encontrado"
        };
    }

    public virtual async Task<ActionResponse<IEnumerable<T>>> GetAsync()
    {
        return new ActionResponse<IEnumerable<T>>
        {
            WasSuccess = true,
            Result = await _entity.ToListAsync()
        };
    }

    public virtual async Task<ActionResponse<T>> UpdateAsync(T entity)
    {
        try
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return new ActionResponse<T>
            {
                WasSuccess = true,
                Result = entity
            };
        }
        catch (DbUpdateException)
        {
            return DbUpdateExceptionActionResponse();
        }
        catch (Exception exception)
        {
            return ExceptionActionResponse(exception);
        }
    }

    private ActionResponse<T> DbUpdateExceptionActionResponse()
    {
        return new ActionResponse<T>
        {
            WasSuccess = false,
            Message = "Ya existe el registro que estas intentando crear."
        };
    }

    private ActionResponse<T> ExceptionActionResponse(Exception exception)
    {
        return new ActionResponse<T>
        {
            WasSuccess = false,
            Message = exception.Message
        };
    }
}