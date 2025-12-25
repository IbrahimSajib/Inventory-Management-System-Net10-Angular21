using InventoryManagement.Application.DTOs.Common;
using InventoryManagement.Application.DTOs.PurchaseOrders;
using InventoryManagement.Application.Mappings;
using InventoryManagement.Core.Common;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Enums;
using InventoryManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Services;

public interface IPurchaseOrderService
{
    Task<ApiResponse<PagedResult<PurchaseOrderDto>>> GetAllOrdersAsync(PaginationParams pagination);
    Task<ApiResponse<PurchaseOrderDto>> GetOrderByIdAsync(int id);
    Task<ApiResponse<PurchaseOrderDto>> CreateOrderAsync(CreatePurchaseOrderDto dto);
    Task<ApiResponse<PurchaseOrderDto>> UpdateOrderAsync(int id, UpdatePurchaseOrderDto dto);
    Task<ApiResponse<bool>> DeleteOrderAsync(int id);
}

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public PurchaseOrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<PurchaseOrderDto>>> GetAllOrdersAsync(PaginationParams pagination)
    {
        var query = from order in _unitOfWork.PurchaseOrders.GetQueryable()
                    select order;

        if (!string.IsNullOrWhiteSpace(pagination.SearchTerm))
        {
            var searchTerm = pagination.SearchTerm.ToLower();
            query = from order in query
                    where order.OrderNumber.ToLower().Contains(searchTerm)
                    select order;
        }

        var totalCount = await query.CountAsync();
        query = query.OrderByDescending(o => o.OrderDate);

        var orders = await query
            .Include(o => o.Vendor)
            .Include(o => o.PurchaseOrderItems)
                .ThenInclude(i => i.Item)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToDto()
            .ToListAsync();

        var result = new PagedResult<PurchaseOrderDto>
        {
            Items = orders,
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };

        return ApiResponse<PagedResult<PurchaseOrderDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<PurchaseOrderDto>> GetOrderByIdAsync(int id)
    {
        var order = await (from o in _unitOfWork.PurchaseOrders.GetQueryable()
                           where o.Id == id
                           select o)
                          .Include(o => o.Vendor)
                          .Include(o => o.PurchaseOrderItems)
                              .ThenInclude(i => i.Item)
                          .FirstOrDefaultAsync();

        if (order == null)
        {
            return ApiResponse<PurchaseOrderDto>.FailureResponse("Purchase order not found");
        }

        return ApiResponse<PurchaseOrderDto>.SuccessResponse(order.ToDto());
    }

    public async Task<ApiResponse<PurchaseOrderDto>> CreateOrderAsync(CreatePurchaseOrderDto dto)
    {
        // Validate vendor
        var vendor = await _unitOfWork.Vendors.GetByIdAsync(dto.VendorId);
        if (vendor == null)
        {
            return ApiResponse<PurchaseOrderDto>.FailureResponse("Invalid vendor");
        }

        // Validate all items
        var itemIds = dto.Items.Select(i => i.ItemId).ToList();
        var items = await (from item in _unitOfWork.Items.GetQueryable()
                           where itemIds.Contains(item.Id)
                           select item).ToListAsync();

        if (items.Count != itemIds.Count)
        {
            return ApiResponse<PurchaseOrderDto>.FailureResponse("One or more invalid items");
        }

        // Generate order number
        var orderNumber = $"PO-{DateTime.UtcNow:yyyyMMddHHmmss}";

        var order = dto.ToEntity(orderNumber);
        await _unitOfWork.PurchaseOrders.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        // Reload with includes
        var createdOrder = await (from o in _unitOfWork.PurchaseOrders.GetQueryable()
                                  where o.Id == order.Id
                                  select o)
                                 .Include(o => o.Vendor)
                                 .Include(o => o.PurchaseOrderItems)
                                     .ThenInclude(i => i.Item)
                                 .FirstAsync();

        return ApiResponse<PurchaseOrderDto>.SuccessResponse(createdOrder.ToDto(), "Purchase order created successfully");
    }

    public async Task<ApiResponse<PurchaseOrderDto>> UpdateOrderAsync(int id, UpdatePurchaseOrderDto dto)
    {
        var order = await (from o in _unitOfWork.PurchaseOrders.GetQueryable()
                           where o.Id == id
                           select o)
                          .Include(o => o.PurchaseOrderItems)
                          .FirstOrDefaultAsync();

        if (order == null)
        {
            return ApiResponse<PurchaseOrderDto>.FailureResponse("Purchase order not found");
        }

        // Validate vendor
        var vendor = await _unitOfWork.Vendors.GetByIdAsync(dto.VendorId);
        if (vendor == null)
        {
            return ApiResponse<PurchaseOrderDto>.FailureResponse("Invalid vendor");
        }

        // Update order header
        dto.UpdateEntity(order);

        // Update items - remove old and add new
        _unitOfWork.PurchaseOrderItems.DeleteRange(order.PurchaseOrderItems);

        foreach (var itemDto in dto.Items)
        {
            var totalPrice = itemDto.Quantity * itemDto.UnitPrice;
            var discountAmount = itemDto.DiscountPercent.HasValue
                ? totalPrice * (itemDto.DiscountPercent.Value / 100)
                : 0;
            var netAmount = totalPrice - discountAmount;

            order.PurchaseOrderItems.Add(new PurchaseOrderItem
            {
                ItemId = itemDto.ItemId,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice,
                TotalPrice = totalPrice,
                DiscountPercent = itemDto.DiscountPercent,
                DiscountAmount = discountAmount,
                NetAmount = netAmount
            });
        }

        _unitOfWork.PurchaseOrders.Update(order);
        await _unitOfWork.SaveChangesAsync();

        // If status is Delivered/Completed, update stock
        if (dto.Status == OrderStatus.Delivered || dto.Status == OrderStatus.Completed)
        {
            foreach (var item in order.PurchaseOrderItems)
            {
                var stock = await (from s in _unitOfWork.ItemStocks.GetQueryable()
                                   where s.ItemId == item.ItemId
                                   select s).FirstOrDefaultAsync();

                if (stock != null)
                {
                    stock.QuantityOnHand += item.Quantity;
                    stock.LastRestockDate = DateTime.UtcNow;
                    _unitOfWork.ItemStocks.Update(stock);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        // Reload with includes
        var updatedOrder = await (from o in _unitOfWork.PurchaseOrders.GetQueryable()
                                  where o.Id == id
                                  select o)
                                 .Include(o => o.Vendor)
                                 .Include(o => o.PurchaseOrderItems)
                                     .ThenInclude(i => i.Item)
                                 .FirstAsync();

        return ApiResponse<PurchaseOrderDto>.SuccessResponse(updatedOrder.ToDto(), "Purchase order updated successfully");
    }

    public async Task<ApiResponse<bool>> DeleteOrderAsync(int id)
    {
        var order = await _unitOfWork.PurchaseOrders.GetByIdAsync(id);

        if (order == null)
        {
            return ApiResponse<bool>.FailureResponse("Purchase order not found");
        }

        _unitOfWork.PurchaseOrders.SoftDelete(order);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Purchase order deleted successfully");
    }
}