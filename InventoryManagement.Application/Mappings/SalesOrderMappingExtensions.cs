using InventoryManagement.Application.DTOs.SalesOrders;
using InventoryManagement.Core.Entities;

namespace InventoryManagement.Application.Mappings;

public static class SalesOrderMappingExtensions
{
    // Entity to DTO
    public static SalesOrderDto ToDto(this SalesOrder order)
    {
        return new SalesOrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer.CustomerName,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            TaxAmount = order.TaxAmount,
            DiscountAmount = order.DiscountAmount,
            GrandTotal = order.GrandTotal,
            Notes = order.Notes,
            ExpectedDeliveryDate = order.ExpectedDeliveryDate,
            ActualDeliveryDate = order.ActualDeliveryDate,
            ShippingAddress = order.ShippingAddress,
            CreatedAt = order.CreatedAt,
            Items = (from item in order.SalesOrderItems
                     select new SalesOrderItemDto
                     {
                         Id = item.Id,
                         ItemId = item.ItemId,
                         ItemName = item.Item.ItemName,
                         ItemCode = item.Item.ItemCode,
                         Quantity = item.Quantity,
                         UnitPrice = item.UnitPrice,
                         TotalPrice = item.TotalPrice,
                         DiscountPercent = item.DiscountPercent,
                         DiscountAmount = item.DiscountAmount,
                         NetAmount = item.NetAmount
                     }).ToList()
        };
    }

    // IQueryable to DTOs using LINQ query syntax
    public static IQueryable<SalesOrderDto> ToDto(this IQueryable<SalesOrder> orders)
    {
        return from order in orders
               select new SalesOrderDto
               {
                   Id = order.Id,
                   OrderNumber = order.OrderNumber,
                   OrderDate = order.OrderDate,
                   CustomerId = order.CustomerId,
                   CustomerName = order.Customer.CustomerName,
                   Status = order.Status,
                   TotalAmount = order.TotalAmount,
                   TaxAmount = order.TaxAmount,
                   DiscountAmount = order.DiscountAmount,
                   GrandTotal = order.GrandTotal,
                   Notes = order.Notes,
                   ExpectedDeliveryDate = order.ExpectedDeliveryDate,
                   ActualDeliveryDate = order.ActualDeliveryDate,
                   ShippingAddress = order.ShippingAddress,
                   CreatedAt = order.CreatedAt,
                   Items = (from item in order.SalesOrderItems
                            select new SalesOrderItemDto
                            {
                                Id = item.Id,
                                ItemId = item.ItemId,
                                ItemName = item.Item.ItemName,
                                ItemCode = item.Item.ItemCode,
                                Quantity = item.Quantity,
                                UnitPrice = item.UnitPrice,
                                TotalPrice = item.TotalPrice,
                                DiscountPercent = item.DiscountPercent,
                                DiscountAmount = item.DiscountAmount,
                                NetAmount = item.NetAmount
                            }).ToList()
               };
    }

    // CreateDto to Entity
    public static SalesOrder ToEntity(this CreateSalesOrderDto dto, string orderNumber)
    {
        var order = new SalesOrder
        {
            OrderNumber = orderNumber,
            OrderDate = dto.OrderDate,
            CustomerId = dto.CustomerId,
            TaxAmount = dto.TaxAmount,
            DiscountAmount = dto.DiscountAmount,
            Notes = dto.Notes,
            ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
            ShippingAddress = dto.ShippingAddress
        };

        // Calculate amounts
        decimal totalAmount = 0;
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

            totalAmount += netAmount;
        }

        order.TotalAmount = totalAmount;
        order.GrandTotal = totalAmount + (dto.TaxAmount ?? 0) - (dto.DiscountAmount ?? 0);

        return order;
    }

    // UpdateDto to Entity
    public static void UpdateEntity(this UpdateSalesOrderDto dto, SalesOrder order)
    {
        order.OrderDate = dto.OrderDate;
        order.CustomerId = dto.CustomerId;
        order.Status = dto.Status;
        order.TaxAmount = dto.TaxAmount;
        order.DiscountAmount = dto.DiscountAmount;
        order.Notes = dto.Notes;
        order.ExpectedDeliveryDate = dto.ExpectedDeliveryDate;
        order.ActualDeliveryDate = dto.ActualDeliveryDate;
        order.ShippingAddress = dto.ShippingAddress;

        // Recalculate amounts
        decimal totalAmount = 0;
        foreach (var itemDto in dto.Items)
        {
            var totalPrice = itemDto.Quantity * itemDto.UnitPrice;
            var discountAmount = itemDto.DiscountPercent.HasValue
                ? totalPrice * (itemDto.DiscountPercent.Value / 100)
                : 0;
            var netAmount = totalPrice - discountAmount;
            totalAmount += netAmount;
        }

        order.TotalAmount = totalAmount;
        order.GrandTotal = totalAmount + (dto.TaxAmount ?? 0) - (dto.DiscountAmount ?? 0);
    }
}