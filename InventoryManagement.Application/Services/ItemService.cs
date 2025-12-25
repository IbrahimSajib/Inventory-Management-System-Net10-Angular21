using InventoryManagement.Application.DTOs.Common;
using InventoryManagement.Application.DTOs.Items;
using InventoryManagement.Application.Mappings;
using InventoryManagement.Core.Common;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Services;

public interface IItemService
{
    Task<ApiResponse<PagedResult<ItemDto>>> GetAllItemsAsync(PaginationParams pagination);
    Task<ApiResponse<List<ItemDto>>> GetAllItemsAsync();
    Task<ApiResponse<ItemDto>> GetItemByIdAsync(int id);
    Task<ApiResponse<ItemDto>> CreateItemAsync(CreateItemDto dto);
    Task<ApiResponse<ItemDto>> UpdateItemAsync(int id, UpdateItemDto dto);
    Task<ApiResponse<bool>> DeleteItemAsync(int id);
    Task<ApiResponse<List<ItemStockDto>>> GetLowStockItemsAsync();
}

public class ItemService : IItemService
{
    private readonly IUnitOfWork _unitOfWork;

    public ItemService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<ItemDto>>> GetAllItemsAsync(PaginationParams pagination)
    {
        var query = from item in _unitOfWork.Items.GetQueryable()
                    select item;

        if (!string.IsNullOrWhiteSpace(pagination.SearchTerm))
        {
            var searchTerm = pagination.SearchTerm.ToLower();
            query = from item in query
                    where item.ItemName.ToLower().Contains(searchTerm) ||
                          item.ItemCode.ToLower().Contains(searchTerm)
                    select item;
        }

        var totalCount = await query.CountAsync();

        query = query.OrderBy(i => i.ItemName);

        var items = await query
            .Include(i => i.Category)
            .Include(i => i.UnitOfMeasure)
            .Include(i => i.ItemStock)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToDto()
            .ToListAsync();

        var result = new PagedResult<ItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };

        return ApiResponse<PagedResult<ItemDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<List<ItemDto>>> GetAllItemsAsync()
    {
        var items = await (from item in _unitOfWork.Items.GetQueryable()
                           orderby item.ItemName
                           select item)
                          .Include(i => i.Category)
                          .Include(i => i.UnitOfMeasure)
                          .Include(i => i.ItemStock)
                          .ToDto()
                          .ToListAsync();

        return ApiResponse<List<ItemDto>>.SuccessResponse(items);
    }

    public async Task<ApiResponse<ItemDto>> GetItemByIdAsync(int id)
    {
        var item = await (from i in _unitOfWork.Items.GetQueryable()
                          select i)
                         .Include(i => i.Category)
                         .Include(i => i.UnitOfMeasure)
                         .Include(i => i.ItemStock)
                         .FirstOrDefaultAsync();

        if (item == null)
        {
            return ApiResponse<ItemDto>.FailureResponse("Item not found");
        }

        return ApiResponse<ItemDto>.SuccessResponse(item.ToDto());
    }

    public async Task<ApiResponse<ItemDto>> CreateItemAsync(CreateItemDto dto)
    {
        var existingItem = await _unitOfWork.Items
            .FirstOrDefaultAsync(i => i.ItemCode == dto.ItemCode);

        if (existingItem != null)
        {
            return ApiResponse<ItemDto>.FailureResponse("Item code already exists");
        }

        // Validate category
        var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
        if (category == null)
        {
            return ApiResponse<ItemDto>.FailureResponse("Invalid category");
        }

        // Validate unit of measure
        var unitOfMeasure = await _unitOfWork.UnitOfMeasures.GetByIdAsync(dto.UnitOfMeasureId);
        if (unitOfMeasure == null)
        {
            return ApiResponse<ItemDto>.FailureResponse("Invalid unit of measure");
        }

        var item = dto.ToEntity();
        await _unitOfWork.Items.AddAsync(item);
        await _unitOfWork.SaveChangesAsync();

        // Create initial stock record
        var itemStock = new ItemStock
        {
            ItemId = item.Id,
            QuantityOnHand = dto.InitialQuantity,
            QuantityReserved = 0,
            LastRestockDate = DateTime.UtcNow
        };

        await _unitOfWork.ItemStocks.AddAsync(itemStock);
        await _unitOfWork.SaveChangesAsync();

        // Reload with includes
        var createdItem = await (from i in _unitOfWork.Items.GetQueryable()
                                 where i.Id == item.Id
                                 select i)
                                .Include(i => i.Category)
                                .Include(i => i.UnitOfMeasure)
                                .Include(i => i.ItemStock)
                                .FirstAsync();

        return ApiResponse<ItemDto>.SuccessResponse(createdItem.ToDto(), "Item created successfully");
    }

    public async Task<ApiResponse<ItemDto>> UpdateItemAsync(int id, UpdateItemDto dto)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(id);

        if (item == null)
        {
            return ApiResponse<ItemDto>.FailureResponse("Item not found");
        }

        if (item.ItemCode != dto.ItemCode)
        {
            var existingItem = await _unitOfWork.Items
                .FirstOrDefaultAsync(i => i.ItemCode == dto.ItemCode);

            if (existingItem != null)
            {
                return ApiResponse<ItemDto>.FailureResponse("Item code already exists");
            }
        }

        // Validate category
        var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
        if (category == null)
        {
            return ApiResponse<ItemDto>.FailureResponse("Invalid category");
        }

        // Validate unit of measure
        var unitOfMeasure = await _unitOfWork.UnitOfMeasures.GetByIdAsync(dto.UnitOfMeasureId);
        if (unitOfMeasure == null)
        {
            return ApiResponse<ItemDto>.FailureResponse("Invalid unit of measure");
        }

        dto.UpdateEntity(item);
        _unitOfWork.Items.Update(item);
        await _unitOfWork.SaveChangesAsync();

        // Reload with includes
        var updatedItem = await (from i in _unitOfWork.Items.GetQueryable()
                                 where i.Id == id
                                 select i)
                                .Include(i => i.Category)
                                .Include(i => i.UnitOfMeasure)
                                .Include(i => i.ItemStock)
                                .FirstAsync();

        return ApiResponse<ItemDto>.SuccessResponse(updatedItem.ToDto(), "Item updated successfully");
    }

    public async Task<ApiResponse<bool>> DeleteItemAsync(int id)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(id);

        if (item == null)
        {
            return ApiResponse<bool>.FailureResponse("Item not found");
        }

        _unitOfWork.Items.SoftDelete(item);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Item deleted successfully");
    }

    public async Task<ApiResponse<List<ItemStockDto>>> GetLowStockItemsAsync()
    {
        var lowStockItems = await (from stock in _unitOfWork.ItemStocks.GetQueryable()
                                   join item in _unitOfWork.Items.GetQueryable() on stock.ItemId equals item.Id
                                   where item.ReorderLevel.HasValue
                                      && stock.QuantityOnHand <= item.ReorderLevel
                                   select stock)
                                  .Include(s => s.Item)
                                  .ToStockDto()
                                  .ToListAsync();

        return ApiResponse<List<ItemStockDto>>.SuccessResponse(lowStockItems);
    }
}