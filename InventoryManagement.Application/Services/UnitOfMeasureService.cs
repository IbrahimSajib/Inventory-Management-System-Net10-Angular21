using InventoryManagement.Application.DTOs.Common;
using InventoryManagement.Application.DTOs.UnitOfMeasures;
using InventoryManagement.Application.Mappings;
using InventoryManagement.Core.Common;
using InventoryManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Services;

public interface IUnitOfMeasureService
{
    Task<ApiResponse<PagedResult<UnitOfMeasureDto>>> GetAllUnitsAsync(PaginationParams pagination);
    Task<ApiResponse<List<UnitOfMeasureDto>>> GetAllUnitsAsync();
    Task<ApiResponse<UnitOfMeasureDto>> GetUnitByIdAsync(int id);
    Task<ApiResponse<UnitOfMeasureDto>> CreateUnitAsync(CreateUnitOfMeasureDto dto);
    Task<ApiResponse<UnitOfMeasureDto>> UpdateUnitAsync(int id, UpdateUnitOfMeasureDto dto);
    Task<ApiResponse<bool>> DeleteUnitAsync(int id);
}

public class UnitOfMeasureService : IUnitOfMeasureService
{
    private readonly IUnitOfWork _unitOfWork;

    public UnitOfMeasureService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<UnitOfMeasureDto>>> GetAllUnitsAsync(PaginationParams pagination)
    {
        var query = from unit in _unitOfWork.UnitOfMeasures.GetQueryable()
                    select unit;

        if (!string.IsNullOrWhiteSpace(pagination.SearchTerm))
        {
            var searchTerm = pagination.SearchTerm.ToLower();
            query = from unit in query
                    where unit.UnitName.ToLower().Contains(searchTerm) ||
                          (unit.ShortName != null && unit.ShortName.ToLower().Contains(searchTerm))
                    select unit;
        }

        var totalCount = await query.CountAsync();
        query = query.OrderBy(u => u.UnitName);

        var units = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToDto()
            .ToListAsync();

        var result = new PagedResult<UnitOfMeasureDto>
        {
            Items = units,
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };

        return ApiResponse<PagedResult<UnitOfMeasureDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<List<UnitOfMeasureDto>>> GetAllUnitsAsync()
    {
        var units = await (from unit in _unitOfWork.UnitOfMeasures.GetQueryable()
                           orderby unit.UnitName
                           select unit)
                          .ToDto()
                          .ToListAsync();

        return ApiResponse<List<UnitOfMeasureDto>>.SuccessResponse(units);
    }

    public async Task<ApiResponse<UnitOfMeasureDto>> GetUnitByIdAsync(int id)
    {
        var unit = await _unitOfWork.UnitOfMeasures.GetByIdAsync(id);

        if (unit == null)
        {
            return ApiResponse<UnitOfMeasureDto>.FailureResponse("Unit of measure not found");
        }

        return ApiResponse<UnitOfMeasureDto>.SuccessResponse(unit.ToDto());
    }

    public async Task<ApiResponse<UnitOfMeasureDto>> CreateUnitAsync(CreateUnitOfMeasureDto dto)
    {
        var existingUnit = await _unitOfWork.UnitOfMeasures
            .FirstOrDefaultAsync(u => u.UnitName == dto.UnitName);

        if (existingUnit != null)
        {
            return ApiResponse<UnitOfMeasureDto>.FailureResponse("Unit name already exists");
        }

        var unit = dto.ToEntity();
        await _unitOfWork.UnitOfMeasures.AddAsync(unit);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<UnitOfMeasureDto>.SuccessResponse(unit.ToDto(), "Unit created successfully");
    }

    public async Task<ApiResponse<UnitOfMeasureDto>> UpdateUnitAsync(int id, UpdateUnitOfMeasureDto dto)
    {
        var unit = await _unitOfWork.UnitOfMeasures.GetByIdAsync(id);

        if (unit == null)
        {
            return ApiResponse<UnitOfMeasureDto>.FailureResponse("Unit of measure not found");
        }

        if (unit.UnitName != dto.UnitName)
        {
            var existingUnit = await _unitOfWork.UnitOfMeasures
                .FirstOrDefaultAsync(u => u.UnitName == dto.UnitName);

            if (existingUnit != null)
            {
                return ApiResponse<UnitOfMeasureDto>.FailureResponse("Unit name already exists");
            }
        }

        dto.UpdateEntity(unit);
        _unitOfWork.UnitOfMeasures.Update(unit);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<UnitOfMeasureDto>.SuccessResponse(unit.ToDto(), "Unit updated successfully");
    }

    public async Task<ApiResponse<bool>> DeleteUnitAsync(int id)
    {
        var unit = await _unitOfWork.UnitOfMeasures.GetByIdAsync(id);

        if (unit == null)
        {
            return ApiResponse<bool>.FailureResponse("Unit of measure not found");
        }

        var hasItems = await (from item in _unitOfWork.Items.GetQueryable()
                              where item.UnitOfMeasureId == id
                              select item).AnyAsync();

        if (hasItems)
        {
            return ApiResponse<bool>.FailureResponse("Cannot delete unit that is assigned to items");
        }

        _unitOfWork.UnitOfMeasures.SoftDelete(unit);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Unit deleted successfully");
    }
}