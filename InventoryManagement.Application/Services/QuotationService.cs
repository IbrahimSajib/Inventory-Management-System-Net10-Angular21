using InventoryManagement.Application.DTOs.Common;
using InventoryManagement.Application.DTOs.Quotations;
using InventoryManagement.Application.Mappings;
using InventoryManagement.Core.Common;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Services;

public interface IQuotationService
{
    Task<ApiResponse<PagedResult<QuotationDto>>> GetAllQuotationsAsync(PaginationParams pagination);
    Task<ApiResponse<QuotationDto>> GetQuotationByIdAsync(int id);
    Task<ApiResponse<QuotationDto>> CreateQuotationAsync(CreateQuotationDto dto);
    Task<ApiResponse<QuotationDto>> UpdateQuotationAsync(int id, UpdateQuotationDto dto);
    Task<ApiResponse<bool>> DeleteQuotationAsync(int id);
}

public class QuotationService : IQuotationService
{
    private readonly IUnitOfWork _unitOfWork;

    public QuotationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<QuotationDto>>> GetAllQuotationsAsync(PaginationParams pagination)
    {
        var query = from quotation in _unitOfWork.Quotations.GetQueryable()
                    select quotation;

        if (!string.IsNullOrWhiteSpace(pagination.SearchTerm))
        {
            var searchTerm = pagination.SearchTerm.ToLower();
            query = from quotation in query
                    where quotation.QuotationNumber.ToLower().Contains(searchTerm)
                    select quotation;
        }

        var totalCount = await query.CountAsync();
        query = query.OrderByDescending(q => q.QuotationDate);

        var quotations = await query
            .Include(q => q.Customer)
            .Include(q => q.QuotationItems)
                .ThenInclude(i => i.Item)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToDto()
            .ToListAsync();

        var result = new PagedResult<QuotationDto>
        {
            Items = quotations,
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };

        return ApiResponse<PagedResult<QuotationDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<QuotationDto>> GetQuotationByIdAsync(int id)
    {
        var quotation = await (from q in _unitOfWork.Quotations.GetQueryable()
                               where q.Id == id
                               select q)
                              .Include(q => q.Customer)
                              .Include(q => q.QuotationItems)
                                  .ThenInclude(i => i.Item)
                              .FirstOrDefaultAsync();

        if (quotation == null)
        {
            return ApiResponse<QuotationDto>.FailureResponse("Quotation not found");
        }

        return ApiResponse<QuotationDto>.SuccessResponse(quotation.ToDto());
    }

    public async Task<ApiResponse<QuotationDto>> CreateQuotationAsync(CreateQuotationDto dto)
    {
        // Validate customer
        var customer = await _unitOfWork.Customers.GetByIdAsync(dto.CustomerId);
        if (customer == null)
        {
            return ApiResponse<QuotationDto>.FailureResponse("Invalid customer");
        }

        // Generate quotation number
        var quotationNumber = $"QT-{DateTime.UtcNow:yyyyMMddHHmmss}";

        var quotation = dto.ToEntity(quotationNumber);
        await _unitOfWork.Quotations.AddAsync(quotation);
        await _unitOfWork.SaveChangesAsync();

        // Reload with includes
        var createdQuotation = await (from q in _unitOfWork.Quotations.GetQueryable()
                                      where q.Id == quotation.Id
                                      select q)
                                     .Include(q => q.Customer)
                                     .Include(q => q.QuotationItems)
                                         .ThenInclude(i => i.Item)
                                     .FirstAsync();

        return ApiResponse<QuotationDto>.SuccessResponse(createdQuotation.ToDto(), "Quotation created successfully");
    }

    public async Task<ApiResponse<QuotationDto>> UpdateQuotationAsync(int id, UpdateQuotationDto dto)
    {
        var quotation = await (from q in _unitOfWork.Quotations.GetQueryable()
                               where q.Id == id
                               select q)
                              .Include(q => q.QuotationItems)
                              .FirstOrDefaultAsync();

        if (quotation == null)
        {
            return ApiResponse<QuotationDto>.FailureResponse("Quotation not found");
        }

        // Validate customer
        var customer = await _unitOfWork.Customers.GetByIdAsync(dto.CustomerId);
        if (customer == null)
        {
            return ApiResponse<QuotationDto>.FailureResponse("Invalid customer");
        }

        // Update quotation
        dto.UpdateEntity(quotation);
        _unitOfWork.QuotationItems.DeleteRange(quotation.QuotationItems);

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
        }

        _unitOfWork.Quotations.Update(quotation);
        await _unitOfWork.SaveChangesAsync();

        // Reload with includes
        var updatedQuotation = await (from q in _unitOfWork.Quotations.GetQueryable()
                                      where q.Id == id
                                      select q)
                                     .Include(q => q.Customer)
                                     .Include(q => q.QuotationItems)
                                         .ThenInclude(i => i.Item)
                                     .FirstAsync();

        return ApiResponse<QuotationDto>.SuccessResponse(updatedQuotation.ToDto(), "Quotation updated successfully");
    }

    public async Task<ApiResponse<bool>> DeleteQuotationAsync(int id)
    {
        var quotation = await _unitOfWork.Quotations.GetByIdAsync(id);

        if (quotation == null)
        {
            return ApiResponse<bool>.FailureResponse("Quotation not found");
        }

        _unitOfWork.Quotations.SoftDelete(quotation);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Quotation deleted successfully");
    }
}