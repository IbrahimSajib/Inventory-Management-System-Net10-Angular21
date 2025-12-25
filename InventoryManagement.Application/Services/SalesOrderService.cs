using InventoryManagement.Application.DTOs.Common;
using InventoryManagement.Application.DTOs.SalesOrders;
using InventoryManagement.Application.Mappings;
using InventoryManagement.Core.Common;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Enums;
using InventoryManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Services;

public interface ISalesOrderService
{
    Task<ApiResponse<PagedResult<SalesOrderDto>>> GetAllOrdersAsync(PaginationParams pagination);
    Task<ApiResponse<SalesOrderDto>> GetOrderByIdAsync(int id);
    Task<ApiResponse<SalesOrderDto>> CreateOrderAsync(CreateSalesOrderDto dto);
    Task<ApiResponse<SalesOrderDto>> UpdateOrderAsync(int id, UpdateSalesOrderDto dto);
    Task<ApiResponse<bool>> DeleteOrderAsync(int id);
}

public class SalesOrderService : ISalesOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public SalesOrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<SalesOrderDto>>> GetAllOrdersAsync(PaginationParams pagination)
    {
        var query = from order in _unitOfWork.SalesOrders.GetQueryable()
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
            .Include(o => o.Customer)
            .Include(o => o.SalesOrderItems)
                .ThenInclude(i => i.Item)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToDto()
            .ToListAsync();

        var result = new PagedResult<SalesOrderDto>
        {
            Items = orders,
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };

        return ApiResponse<PagedResult<SalesOrderDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<SalesOrderDto>> GetOrderByIdAsync(int id)
    {
        var order = await (from o in _unitOfWork.SalesOrders.GetQueryable()
                           where o.Id == id
                           select o)
                          .Include(o => o.Customer)
                          .Include(o => o.SalesOrderItems)
                              .ThenInclude(i => i.Item)
                          .FirstOrDefaultAsync();

        if (order == null)
        {
            return ApiResponse<SalesOrderDto>.FailureResponse("Sales order not found");
        }

        return ApiResponse<SalesOrderDto>.SuccessResponse(order.ToDto());
    }

    public async Task<ApiResponse<SalesOrderDto>> CreateOrderAsync(CreateSalesOrderDto dto)
    {
        // Validate customer
        var customer = await _unitOfWork.Customers.GetByIdAsync(dto.CustomerId);
        if (customer == null)
        {
            return ApiResponse<SalesOrderDto>.FailureResponse("Invalid customer");
        }

        // Validate items and check stock
        foreach (var itemDto in dto.Items)
        {
            var stock = await (from s in _unitOfWork.ItemStocks.GetQueryable()
                               where s.ItemId == itemDto.ItemId
                               select s).FirstOrDefaultAsync();

            if (stock == null)
            {
                return ApiResponse<SalesOrderDto>.FailureResponse($"Item {itemDto.ItemId} not found");
            }

            if (stock.QuantityAvailable < itemDto.Quantity)
            {
                return ApiResponse<SalesOrderDto>.FailureResponse($"Insufficient stock for item {itemDto.ItemId}");
            }
        }

        // Generate order number
        var orderNumber = $"SO-{DateTime.UtcNow:yyyyMMddHHmmss}";

        var order = dto.ToEntity(orderNumber);
        await _unitOfWork.SalesOrders.AddAsync(order);

        // Reserve stock
        foreach (var item in order.SalesOrderItems)
        {
            var stock = await (from s in _unitOfWork.ItemStocks.GetQueryable()
                               where s.ItemId == item.ItemId
                               select s).FirstAsync();

            stock.QuantityReserved += item.Quantity;
            _unitOfWork.ItemStocks.Update(stock);
        }

        await _unitOfWork.SaveChangesAsync();

        // Reload with includes
        var createdOrder = await (from o in _unitOfWork.SalesOrders.GetQueryable()
                                  where o.Id == order.Id
                                  select o)
                                 .Include(o => o.Customer)
                                 .Include(o => o.SalesOrderItems)
                                     .ThenInclude(i => i.Item)
                                 .FirstAsync();

        return ApiResponse<SalesOrderDto>.SuccessResponse(createdOrder.ToDto(), "Sales order created successfully");
    }

    public async Task<ApiResponse<SalesOrderDto>> UpdateOrderAsync(int id, UpdateSalesOrderDto dto)
    {
        var order = await (from o in _unitOfWork.SalesOrders.GetQueryable()
                           where o.Id == id
                           select o)
                          .Include(o => o.SalesOrderItems)
                          .FirstOrDefaultAsync();

        if (order == null)
        {
            return ApiResponse<SalesOrderDto>.FailureResponse("Sales order not found");
        }

        // Release old reserved stock
        foreach (var item in order.SalesOrderItems)
        {
            var stock = await (from s in _unitOfWork.ItemStocks.GetQueryable()
                               where s.ItemId == item.ItemId
                               select s).FirstOrDefaultAsync();

            if (stock != null)
            {
                stock.QuantityReserved -= item.Quantity;
                _unitOfWork.ItemStocks.Update(stock);
            }
        }

        // Update order
        dto.UpdateEntity(order);
        _unitOfWork.SalesOrderItems.DeleteRange(order.SalesOrderItems);

        foreach (var itemDto in dto.Items)
        {
            var totalPrice = itemDto.Quantity * itemDto.UnitPrice;
            var discountAmount = itemDto.DiscountPercent.HasValue
                ? totalPrice * (itemDto.DiscountPercent.Value / 100)
                : 0;
            var netAmount = totalPrice - discountAmount;

            order.SalesOrderItems.Add(new SalesOrderItem
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

        // Reserve new stock or complete order
        if (dto.Status == OrderStatus.Delivered || dto.Status == OrderStatus.Completed)
        {
            foreach (var item in order.SalesOrderItems)
            {
                var stock = await (from s in _unitOfWork.ItemStocks.GetQueryable()
                                   where s.ItemId == item.ItemId
                                   select s).FirstAsync();

                stock.QuantityOnHand -= item.Quantity;
                _unitOfWork.ItemStocks.Update(stock);
            }
        }
        else
        {
            foreach (var item in order.SalesOrderItems)
            {
                var stock = await (from s in _unitOfWork.ItemStocks.GetQueryable()
                                   where s.ItemId == item.ItemId
                                   select s).FirstAsync();

                stock.QuantityReserved += item.Quantity;
                _unitOfWork.ItemStocks.Update(stock);
            }
        }

        _unitOfWork.SalesOrders.Update(order);
        await _unitOfWork.SaveChangesAsync();

        // Reload with includes
        var updatedOrder = await (from o in _unitOfWork.SalesOrders.GetQueryable()
                                  where o.Id == id
                                  select o)
                                 .Include(o => o.Customer)
                                 .Include(o => o.SalesOrderItems)
                                     .ThenInclude(i => i.Item)
                                 .FirstAsync();

        return ApiResponse<SalesOrderDto>.SuccessResponse(updatedOrder.ToDto(), "Sales order updated successfully");
    }

    public async Task<ApiResponse<bool>> DeleteOrderAsync(int id)
    {
        var order = await (from o in _unitOfWork.SalesOrders.GetQueryable()
                           where o.Id == id
                           select o)
                          .Include(o => o.SalesOrderItems)
                          .FirstOrDefaultAsync();

        if (order == null)
        {
            return ApiResponse<bool>.FailureResponse("Sales order not found");
        }

        // Release reserved stock
        foreach (var item in order.SalesOrderItems)
        {
            var stock = await (from s in _unitOfWork.ItemStocks.GetQueryable()
                               where s.ItemId == item.ItemId
                               select s).FirstOrDefaultAsync();

            if (stock != null)
            {
                stock.QuantityReserved -= item.Quantity;
                _unitOfWork.ItemStocks.Update(stock);
            }
        }

        _unitOfWork.SalesOrders.SoftDelete(order);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Sales order deleted successfully");
    }
}