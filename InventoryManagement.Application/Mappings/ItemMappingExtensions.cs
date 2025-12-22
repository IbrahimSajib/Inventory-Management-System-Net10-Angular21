using InventoryManagement.Application.DTOs.Items;
using InventoryManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Mappings;

public static class ItemMappingExtensions
{
    // Entity to DTO (with stock info)
    public static ItemDto ToDto(this Item item)
    {
        return new ItemDto
        {
            Id = item.Id,
            ItemCode = item.ItemCode,
            ItemName = item.ItemName,
            Description = item.Description,
            CategoryId = item.CategoryId,
            CategoryName = item.Category.CategoryName,
            UnitOfMeasureId = item.UnitOfMeasureId,
            UnitOfMeasureName = item.UnitOfMeasure.UnitName,
            UnitPrice = item.UnitPrice,
            ReorderLevel = item.ReorderLevel,
            ImageUrl = item.ImageUrl,
            QuantityOnHand = item.ItemStock?.QuantityOnHand ?? 0,
            QuantityAvailable = item.ItemStock?.QuantityAvailable ?? 0,
            CreatedAt = item.CreatedAt
        };
    }

    // IQueryable to DTOs using LINQ query syntax
    public static IQueryable<ItemDto> ToDto(this IQueryable<Item> items)
    {
        return from item in items
               select new ItemDto
               {
                   Id = item.Id,
                   ItemCode = item.ItemCode,
                   ItemName = item.ItemName,
                   Description = item.Description,
                   CategoryId = item.CategoryId,
                   CategoryName = item.Category.CategoryName,
                   UnitOfMeasureId = item.UnitOfMeasureId,
                   UnitOfMeasureName = item.UnitOfMeasure.UnitName,
                   UnitPrice = item.UnitPrice,
                   ReorderLevel = item.ReorderLevel,
                   ImageUrl = item.ImageUrl,
                   QuantityOnHand = item.ItemStock != null ? item.ItemStock.QuantityOnHand : 0,
                   QuantityAvailable = item.ItemStock != null ? item.ItemStock.QuantityAvailable : 0,
                   CreatedAt = item.CreatedAt
               };
    }

    // CreateDto to Entity
    public static Item ToEntity(this CreateItemDto dto)
    {
        return new Item
        {
            ItemCode = dto.ItemCode,
            ItemName = dto.ItemName,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            UnitOfMeasureId = dto.UnitOfMeasureId,
            UnitPrice = dto.UnitPrice,
            ReorderLevel = dto.ReorderLevel,
            ImageUrl = dto.ImageUrl
        };
    }

    // UpdateDto to Entity
    public static void UpdateEntity(this UpdateItemDto dto, Item item)
    {
        item.ItemCode = dto.ItemCode;
        item.ItemName = dto.ItemName;
        item.Description = dto.Description;
        item.CategoryId = dto.CategoryId;
        item.UnitOfMeasureId = dto.UnitOfMeasureId;
        item.UnitPrice = dto.UnitPrice;
        item.ReorderLevel = dto.ReorderLevel;
        item.ImageUrl = dto.ImageUrl;
    }

    // ItemStock to DTO
    public static ItemStockDto ToStockDto(this ItemStock stock)
    {
        return new ItemStockDto
        {
            Id = stock.Id,
            ItemId = stock.ItemId,
            ItemName = stock.Item.ItemName,
            ItemCode = stock.Item.ItemCode,
            QuantityOnHand = stock.QuantityOnHand,
            QuantityReserved = stock.QuantityReserved,
            QuantityAvailable = stock.QuantityAvailable,
            LastRestockDate = stock.LastRestockDate
        };
    }

    // IQueryable ItemStock to DTOs using LINQ query syntax
    public static IQueryable<ItemStockDto> ToStockDto(this IQueryable<ItemStock> stocks)
    {
        return from stock in stocks
               select new ItemStockDto
               {
                   Id = stock.Id,
                   ItemId = stock.ItemId,
                   ItemName = stock.Item.ItemName,
                   ItemCode = stock.Item.ItemCode,
                   QuantityOnHand = stock.QuantityOnHand,
                   QuantityReserved = stock.QuantityReserved,
                   QuantityAvailable = stock.QuantityAvailable,
                   LastRestockDate = stock.LastRestockDate
               };
    }
}