using InventoryManagement.Application.DTOs.PurchaseOrders;
using InventoryManagement.Core.Entities;

namespace InventoryManagement.Application.Mappings;

public static class PurchaseOrderMappingExtensions
{
    // Entity to DTO
    public static PurchaseOrderDto ToDto(this PurchaseOrder order)
    {
        return new PurchaseOrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
            VendorId = order.VendorId,
            VendorName = order.Vendor.VendorName,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            TaxAmount = order.TaxAmount,
            DiscountAmount = order.DiscountAmount,
            GrandTotal = order.GrandTotal,
            Notes = order.Notes,
            ExpectedDeliveryDate = order.ExpectedDeliveryDate,
            ActualDeliveryDate = order.ActualDeliveryDate,
            CreatedAt = order.CreatedAt,
            Items = (from item in order.PurchaseOrderItems
                     select new PurchaseOrderItemDto
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
    public static IQueryable<PurchaseOrderDto> ToDto(this IQueryable<PurchaseOrder> orders)
    {
        return from order in orders
               select new PurchaseOrderDto
               {
                   Id = order.Id,
                   OrderNumber = order.OrderNumber,
                   OrderDate = order.OrderDate,
                   VendorId = order.VendorId,
                   VendorName = order.Vendor.VendorName,
                   Status = order.Status,
                   TotalAmount = order.TotalAmount,
                   TaxAmount = order.TaxAmount,
                   DiscountAmount = order.DiscountAmount,
                   GrandTotal = order.GrandTotal,
                   Notes = order.Notes,
                   ExpectedDeliveryDate = order.ExpectedDeliveryDate,
                   ActualDeliveryDate = order.ActualDeliveryDate,
                   CreatedAt = order.CreatedAt,
                   Items = (from item in order.PurchaseOrderItems
                            select new PurchaseOrderItemDto
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
    public static PurchaseOrder ToEntity(this CreatePurchaseOrderDto dto, string orderNumber)
    {
        var order = new PurchaseOrder
        {
            OrderNumber = orderNumber,
            OrderDate = dto.OrderDate,
            VendorId = dto.VendorId,
            TaxAmount = dto.TaxAmount,
            DiscountAmount = dto.DiscountAmount,
            Notes = dto.Notes,
            ExpectedDeliveryDate = dto.ExpectedDeliveryDate
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

            totalAmount += netAmount;
        }

        order.TotalAmount = totalAmount;
        order.GrandTotal = totalAmount + (dto.TaxAmount ?? 0) - (dto.DiscountAmount ?? 0);

        return order;
    }

    // UpdateDto to Entity
    public static void UpdateEntity(this UpdatePurchaseOrderDto dto, PurchaseOrder order)
    {
        order.OrderDate = dto.OrderDate;
        order.VendorId = dto.VendorId;
        order.Status = dto.Status;
        order.TaxAmount = dto.TaxAmount;
        order.DiscountAmount = dto.DiscountAmount;
        order.Notes = dto.Notes;
        order.ExpectedDeliveryDate = dto.ExpectedDeliveryDate;
        order.ActualDeliveryDate = dto.ActualDeliveryDate;

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