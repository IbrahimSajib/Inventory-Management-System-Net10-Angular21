using InventoryManagement.Application.DTOs.Categories;
using InventoryManagement.Core.Entities;

namespace InventoryManagement.Application.Mappings;

public static class CategoryMappingExtensions
{
    // Entity to DTO
    public static CategoryDto ToDto(this Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            CategoryName = category.CategoryName,
            Description = category.Description,
            CreatedAt = category.CreatedAt
        };
    }

    // IQueryable to DTOs using LINQ query syntax
    public static IQueryable<CategoryDto> ToDto(this IQueryable<Category> categories)
    {
        return from category in categories
               select new CategoryDto
               {
                   Id = category.Id,
                   CategoryName = category.CategoryName,
                   Description = category.Description,
                   CreatedAt = category.CreatedAt
               };
    }

    // CreateDto to Entity
    public static Category ToEntity(this CreateCategoryDto dto)
    {
        return new Category
        {
            CategoryName = dto.CategoryName,
            Description = dto.Description
        };
    }

    // UpdateDto to Entity
    public static void UpdateEntity(this UpdateCategoryDto dto, Category category)
    {
        category.CategoryName = dto.CategoryName;
        category.Description = dto.Description;
    }
}