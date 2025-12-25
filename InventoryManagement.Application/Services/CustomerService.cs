using InventoryManagement.Application.DTOs.Common;
using InventoryManagement.Application.DTOs.Customers;
using InventoryManagement.Application.Mappings;
using InventoryManagement.Core.Common;
using InventoryManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Services;

public interface ICustomerService
{
    Task<ApiResponse<PagedResult<CustomerDto>>> GetAllCustomersAsync(PaginationParams pagination);
    Task<ApiResponse<List<CustomerDto>>> GetAllCustomersAsync();
    Task<ApiResponse<CustomerDto>> GetCustomerByIdAsync(int id);
    Task<ApiResponse<CustomerDto>> CreateCustomerAsync(CreateCustomerDto dto);
    Task<ApiResponse<CustomerDto>> UpdateCustomerAsync(int id, UpdateCustomerDto dto);
    Task<ApiResponse<bool>> DeleteCustomerAsync(int id);
}

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<CustomerDto>>> GetAllCustomersAsync(PaginationParams pagination)
    {
        var query = from customer in _unitOfWork.Customers.GetQueryable()
                    select customer;

        if (!string.IsNullOrWhiteSpace(pagination.SearchTerm))
        {
            var searchTerm = pagination.SearchTerm.ToLower();
            query = from customer in query
                    where customer.CustomerName.ToLower().Contains(searchTerm) ||
                          (customer.Email != null && customer.Email.ToLower().Contains(searchTerm)) ||
                          (customer.PhoneNumber != null && customer.PhoneNumber.Contains(searchTerm))
                    select customer;
        }

        var totalCount = await query.CountAsync();

        query = query.OrderBy(c => c.CustomerName);

        var customers = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToDto()
            .ToListAsync();

        var result = new PagedResult<CustomerDto>
        {
            Items = customers,
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };

        return ApiResponse<PagedResult<CustomerDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<List<CustomerDto>>> GetAllCustomersAsync()
    {
        var customers = await (from customer in _unitOfWork.Customers.GetQueryable()
                               orderby customer.CustomerName
                               select customer)
                              .ToDto()
                              .ToListAsync();

        return ApiResponse<List<CustomerDto>>.SuccessResponse(customers);
    }

    public async Task<ApiResponse<CustomerDto>> GetCustomerByIdAsync(int id)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id);

        if (customer == null)
        {
            return ApiResponse<CustomerDto>.FailureResponse("Customer not found");
        }

        return ApiResponse<CustomerDto>.SuccessResponse(customer.ToDto());
    }

    public async Task<ApiResponse<CustomerDto>> CreateCustomerAsync(CreateCustomerDto dto)
    {
        var customer = dto.ToEntity();
        await _unitOfWork.Customers.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<CustomerDto>.SuccessResponse(customer.ToDto(), "Customer created successfully");
    }

    public async Task<ApiResponse<CustomerDto>> UpdateCustomerAsync(int id, UpdateCustomerDto dto)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id);

        if (customer == null)
        {
            return ApiResponse<CustomerDto>.FailureResponse("Customer not found");
        }

        dto.UpdateEntity(customer);
        _unitOfWork.Customers.Update(customer);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<CustomerDto>.SuccessResponse(customer.ToDto(), "Customer updated successfully");
    }

    public async Task<ApiResponse<bool>> DeleteCustomerAsync(int id)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id);

        if (customer == null)
        {
            return ApiResponse<bool>.FailureResponse("Customer not found");
        }

        _unitOfWork.Customers.SoftDelete(customer);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Customer deleted successfully");
    }
}