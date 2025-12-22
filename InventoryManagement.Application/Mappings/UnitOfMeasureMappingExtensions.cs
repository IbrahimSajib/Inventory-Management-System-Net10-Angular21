using InventoryManagement.Application.DTOs.UnitOfMeasures;
using InventoryManagement.Core.Entities;

namespace InventoryManagement.Application.Mappings;

public static class UnitOfMeasureMappingExtensions
{
    // Entity to DTO
    public static UnitOfMeasureDto ToDto(this UnitOfMeasure unitOfMeasure)
    {
        return new UnitOfMeasureDto
        {
            Id = unitOfMeasure.Id,
            UnitName = unitOfMeasure.UnitName,
            ShortName = unitOfMeasure.ShortName,
            Description = unitOfMeasure.Description,
            CreatedAt = unitOfMeasure.CreatedAt
        };
    }

    // IQueryable to DTOs using LINQ query syntax
    public static IQueryable<UnitOfMeasureDto> ToDto(this IQueryable<UnitOfMeasure> unitOfMeasures)
    {
        return from uom in unitOfMeasures
               select new UnitOfMeasureDto
               {
                   Id = uom.Id,
                   UnitName = uom.UnitName,
                   ShortName = uom.ShortName,
                   Description = uom.Description,
                   CreatedAt = uom.CreatedAt
               };
    }

    // CreateDto to Entity
    public static UnitOfMeasure ToEntity(this CreateUnitOfMeasureDto dto)
    {
        return new UnitOfMeasure
        {
            UnitName = dto.UnitName,
            ShortName = dto.ShortName,
            Description = dto.Description
        };
    }

    // UpdateDto to Entity
    public static void UpdateEntity(this UpdateUnitOfMeasureDto dto, UnitOfMeasure unitOfMeasure)
    {
        unitOfMeasure.UnitName = dto.UnitName;
        unitOfMeasure.ShortName = dto.ShortName;
        unitOfMeasure.Description = dto.Description;
    }
}