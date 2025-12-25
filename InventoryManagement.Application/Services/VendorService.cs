using InventoryManagement.Application.DTOs.Common;
using InventoryManagement.Application.DTOs.Vendors;
using InventoryManagement.Application.Mappings;
using InventoryManagement.Core.Common;
using InventoryManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Services;

public interface IVendorService
{
    Task<ApiResponse<PagedResult<VendorDto>>> GetAllVendorsAsync(PaginationParams pagination);
    Task<ApiResponse<List<VendorDto>>> GetAllVendorsAsync();
    Task<ApiResponse<VendorDto>> GetVendorByIdAsync(int id);
    Task<ApiResponse<VendorDto>> CreateVendorAsync(CreateVendorDto dto);
    Task<ApiResponse<VendorDto>> UpdateVendorAsync(int id, UpdateVendorDto dto);
    Task<ApiResponse<bool>> DeleteVendorAsync(int id);
}

public class VendorService : IVendorService
{
    private readonly IUnitOfWork _unitOfWork;

    public VendorService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<VendorDto>>> GetAllVendorsAsync(PaginationParams pagination)
    {
        var query = from vendor in _unitOfWork.Vendors.GetQueryable()
                    select vendor;

        if (!string.IsNullOrWhiteSpace(pagination.SearchTerm))
        {
            var searchTerm = pagination.SearchTerm.ToLower();
            query = from vendor in query
                    where vendor.VendorName.ToLower().Contains(searchTerm) ||
                          (vendor.Email != null && vendor.Email.ToLower().Contains(searchTerm)) ||
                          (vendor.PhoneNumber != null && vendor.PhoneNumber.Contains(searchTerm))
                    select vendor;
        }

        var totalCount = await query.CountAsync();

        query = query.OrderBy(v => v.VendorName);

        var vendors = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToDto()
            .ToListAsync();

        var result = new PagedResult<VendorDto>
        {
            Items = vendors,
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };

        return ApiResponse<PagedResult<VendorDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<List<VendorDto>>> GetAllVendorsAsync()
    {
        var vendors = await (from vendor in _unitOfWork.Vendors.GetQueryable()
                             orderby vendor.VendorName
                             select vendor)
                            .ToDto()
                            .ToListAsync();

        return ApiResponse<List<VendorDto>>.SuccessResponse(vendors);
    }

    public async Task<ApiResponse<VendorDto>> GetVendorByIdAsync(int id)
    {
        var vendor = await _unitOfWork.Vendors.GetByIdAsync(id);

        if (vendor == null)
        {
            return ApiResponse<VendorDto>.FailureResponse("Vendor not found");
        }

        return ApiResponse<VendorDto>.SuccessResponse(vendor.ToDto());
    }

    public async Task<ApiResponse<VendorDto>> CreateVendorAsync(CreateVendorDto dto)
    {
        var vendor = dto.ToEntity();
        await _unitOfWork.Vendors.AddAsync(vendor);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<VendorDto>.SuccessResponse(vendor.ToDto(), "Vendor created successfully");
    }

    public async Task<ApiResponse<VendorDto>> UpdateVendorAsync(int id, UpdateVendorDto dto)
    {
        var vendor = await _unitOfWork.Vendors.GetByIdAsync(id);

        if (vendor == null)
        {
            return ApiResponse<VendorDto>.FailureResponse("Vendor not found");
        }

        dto.UpdateEntity(vendor);
        _unitOfWork.Vendors.Update(vendor);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<VendorDto>.SuccessResponse(vendor.ToDto(), "Vendor updated successfully");
    }

    public async Task<ApiResponse<bool>> DeleteVendorAsync(int id)
    {
        var vendor = await _unitOfWork.Vendors.GetByIdAsync(id);

        if (vendor == null)
        {
            return ApiResponse<bool>.FailureResponse("Vendor not found");
        }

        _unitOfWork.Vendors.SoftDelete(vendor);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Vendor deleted successfully");
    }
}