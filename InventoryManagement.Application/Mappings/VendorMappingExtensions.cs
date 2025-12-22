using InventoryManagement.Application.DTOs.Vendors;
using InventoryManagement.Core.Entities;

namespace InventoryManagement.Application.Mappings;

public static class VendorMappingExtensions
{
    // Entity to DTO
    public static VendorDto ToDto(this Vendor vendor)
    {
        return new VendorDto
        {
            Id = vendor.Id,
            VendorName = vendor.VendorName,
            Email = vendor.Email,
            PhoneNumber = vendor.PhoneNumber,
            Address = vendor.Address,
            City = vendor.City,
            Country = vendor.Country,
            PostalCode = vendor.PostalCode,
            CreatedAt = vendor.CreatedAt
        };
    }

    // IQueryable to DTOs using LINQ query syntax
    public static IQueryable<VendorDto> ToDto(this IQueryable<Vendor> vendors)
    {
        return from vendor in vendors
               select new VendorDto
               {
                   Id = vendor.Id,
                   VendorName = vendor.VendorName,
                   Email = vendor.Email,
                   PhoneNumber = vendor.PhoneNumber,
                   Address = vendor.Address,
                   City = vendor.City,
                   Country = vendor.Country,
                   PostalCode = vendor.PostalCode,
                   CreatedAt = vendor.CreatedAt
               };
    }

    // CreateDto to Entity
    public static Vendor ToEntity(this CreateVendorDto dto)
    {
        return new Vendor
        {
            VendorName = dto.VendorName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            City = dto.City,
            Country = dto.Country,
            PostalCode = dto.PostalCode
        };
    }

    // UpdateDto to Entity
    public static void UpdateEntity(this UpdateVendorDto dto, Vendor vendor)
    {
        vendor.VendorName = dto.VendorName;
        vendor.Email = dto.Email;
        vendor.PhoneNumber = dto.PhoneNumber;
        vendor.Address = dto.Address;
        vendor.City = dto.City;
        vendor.Country = dto.Country;
        vendor.PostalCode = dto.PostalCode;
    }
}