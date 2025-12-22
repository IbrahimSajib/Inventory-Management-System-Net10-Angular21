using InventoryManagement.Application.DTOs.Quotations;
using InventoryManagement.Core.Entities;

namespace InventoryManagement.Application.Mappings;

public static class QuotationMappingExtensions
{
    // Entity to DTO
    public static QuotationDto ToDto(this Quotation quotation)
    {
        return new QuotationDto
        {
            Id = quotation.Id,
            QuotationNumber = quotation.QuotationNumber,
            QuotationDate = quotation.QuotationDate,
            ValidUntil = quotation.ValidUntil,
            CustomerId = quotation.CustomerId,
            CustomerName = quotation.Customer.CustomerName,
            Status = quotation.Status,
            TotalAmount = quotation.TotalAmount,
            TaxAmount = quotation.TaxAmount,
            DiscountAmount = quotation.DiscountAmount,
            GrandTotal = quotation.GrandTotal,
            Notes = quotation.Notes,
            TermsAndConditions = quotation.TermsAndConditions,
            CreatedAt = quotation.CreatedAt,
            Items = (from item in quotation.QuotationItems
                     select new QuotationItemDto
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
                         NetAmount = item.NetAmount,
                         Description = item.Description
                     }).ToList()
        };
    }

    // IQueryable to DTOs using LINQ query syntax
    public static IQueryable<QuotationDto> ToDto(this IQueryable<Quotation> quotations)
    {
        return from quotation in quotations
               select new QuotationDto
               {
                   Id = quotation.Id,
                   QuotationNumber = quotation.QuotationNumber,
                   QuotationDate = quotation.QuotationDate,
                   ValidUntil = quotation.ValidUntil,
                   CustomerId = quotation.CustomerId,
                   CustomerName = quotation.Customer.CustomerName,
                   Status = quotation.Status,
                   TotalAmount = quotation.TotalAmount,
                   TaxAmount = quotation.TaxAmount,
                   DiscountAmount = quotation.DiscountAmount,
                   GrandTotal = quotation.GrandTotal,
                   Notes = quotation.Notes,
                   TermsAndConditions = quotation.TermsAndConditions,
                   CreatedAt = quotation.CreatedAt,
                   Items = (from item in quotation.QuotationItems
                            select new QuotationItemDto
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
                                NetAmount = item.NetAmount,
                                Description = item.Description
                            }).ToList()
               };
    }

    // CreateDto to Entity
    public static Quotation ToEntity(this CreateQuotationDto dto, string quotationNumber)
    {
        var quotation = new Quotation
        {
            QuotationNumber = quotationNumber,
            QuotationDate = dto.QuotationDate,
            ValidUntil = dto.ValidUntil,
            CustomerId = dto.CustomerId,
            TaxAmount = dto.TaxAmount,
            DiscountAmount = dto.DiscountAmount,
            Notes = dto.Notes,
            TermsAndConditions = dto.TermsAndConditions
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

            quotation.QuotationItems.Add(new QuotationItem
            {
                ItemId = itemDto.ItemId,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice,
                TotalPrice = totalPrice,
                DiscountPercent = itemDto.DiscountPercent,
                DiscountAmount = discountAmount,
                NetAmount = netAmount,
                Description = itemDto.Description
            });

            totalAmount += netAmount;
        }

        quotation.TotalAmount = totalAmount;
        quotation.GrandTotal = totalAmount + (dto.TaxAmount ?? 0) - (dto.DiscountAmount ?? 0);

        return quotation;
    }

    // UpdateDto to Entity
    public static void UpdateEntity(this UpdateQuotationDto dto, Quotation quotation)
    {
        quotation.QuotationDate = dto.QuotationDate;
        quotation.ValidUntil = dto.ValidUntil;
        quotation.CustomerId = dto.CustomerId;
        quotation.Status = dto.Status;
        quotation.TaxAmount = dto.TaxAmount;
        quotation.DiscountAmount = dto.DiscountAmount;
        quotation.Notes = dto.Notes;
        quotation.TermsAndConditions = dto.TermsAndConditions;

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

        quotation.TotalAmount = totalAmount;
        quotation.GrandTotal = totalAmount + (dto.TaxAmount ?? 0) - (dto.DiscountAmount ?? 0);
    }
}