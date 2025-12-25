using InventoryManagement.Application.DTOs.Categories;
using InventoryManagement.Application.DTOs.Common;
using InventoryManagement.Application.Mappings;
using InventoryManagement.Core.Common;
using InventoryManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Services;

public interface ICategoryService
{
    Task<ApiResponse<PagedResult<CategoryDto>>> GetAllCategoriesAsync(PaginationParams pagination);
    Task<ApiResponse<List<CategoryDto>>> GetAllCategoriesAsync();
    Task<ApiResponse<CategoryDto>> GetCategoryByIdAsync(int id);
    Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto);
    Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
    Task<ApiResponse<bool>> DeleteCategoryAsync(int id);
}

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<CategoryDto>>> GetAllCategoriesAsync(PaginationParams pagination)
    {
        var query = from category in _unitOfWork.Categories.GetQueryable()
                    select category;

        if (!string.IsNullOrWhiteSpace(pagination.SearchTerm))
        {
            var searchTerm = pagination.SearchTerm.ToLower();
            query = from category in query
                    where category.CategoryName.ToLower().Contains(searchTerm) ||
                          (category.Description != null && category.Description.ToLower().Contains(searchTerm))
                    select category;
        }

        var totalCount = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(pagination.SortBy))
        {
            query = pagination.SortBy.ToLower() switch
            {
                "categoryname" => pagination.SortDescending
                    ? query.OrderByDescending(c => c.CategoryName)
                    : query.OrderBy(c => c.CategoryName),
                "createdat" => pagination.SortDescending
                    ? query.OrderByDescending(c => c.CreatedAt)
                    : query.OrderBy(c => c.CreatedAt),
                _ => query.OrderBy(c => c.Id)
            };
        }
        else
        {
            query = query.OrderBy(c => c.CategoryName);
        }

        var categories = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToDto()
            .ToListAsync();

        var result = new PagedResult<CategoryDto>
        {
            Items = categories,
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };

        return ApiResponse<PagedResult<CategoryDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<List<CategoryDto>>> GetAllCategoriesAsync()
    {
        var categories = await (from category in _unitOfWork.Categories.GetQueryable()
                                orderby category.CategoryName
                                select category)
                               .ToDto()
                               .ToListAsync();

        return ApiResponse<List<CategoryDto>>.SuccessResponse(categories);
    }

    public async Task<ApiResponse<CategoryDto>> GetCategoryByIdAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);

        if (category == null)
        {
            return ApiResponse<CategoryDto>.FailureResponse("Category not found");
        }

        return ApiResponse<CategoryDto>.SuccessResponse(category.ToDto());
    }

    public async Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var existingCategory = await _unitOfWork.Categories
            .FirstOrDefaultAsync(c => c.CategoryName == dto.CategoryName);

        if (existingCategory != null)
        {
            return ApiResponse<CategoryDto>.FailureResponse("Category name already exists");
        }

        var category = dto.ToEntity();
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<CategoryDto>.SuccessResponse(category.ToDto(), "Category created successfully");
    }

    public async Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);

        if (category == null)
        {
            return ApiResponse<CategoryDto>.FailureResponse("Category not found");
        }

        if (category.CategoryName != dto.CategoryName)
        {
            var existingCategory = await _unitOfWork.Categories
                .FirstOrDefaultAsync(c => c.CategoryName == dto.CategoryName);

            if (existingCategory != null)
            {
                return ApiResponse<CategoryDto>.FailureResponse("Category name already exists");
            }
        }

        dto.UpdateEntity(category);
        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<CategoryDto>.SuccessResponse(category.ToDto(), "Category updated successfully");
    }

    public async Task<ApiResponse<bool>> DeleteCategoryAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);

        if (category == null)
        {
            return ApiResponse<bool>.FailureResponse("Category not found");
        }

        var hasItems = await (from item in _unitOfWork.Items.GetQueryable()
                              where item.CategoryId == id
                              select item).AnyAsync();

        if (hasItems)
        {
            return ApiResponse<bool>.FailureResponse("Cannot delete category that is assigned to items");
        }

        _unitOfWork.Categories.SoftDelete(category);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Category deleted successfully");
    }
}