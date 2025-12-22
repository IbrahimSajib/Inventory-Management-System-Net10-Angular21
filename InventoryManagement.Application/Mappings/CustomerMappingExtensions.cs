using InventoryManagement.Application.DTOs.Customers;
using InventoryManagement.Core.Entities;

namespace InventoryManagement.Application.Mappings;

public static class CustomerMappingExtensions
{
    // Entity to DTO
    public static CustomerDto ToDto(this Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            CustomerName = customer.CustomerName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Address = customer.Address,
            City = customer.City,
            Country = customer.Country,
            PostalCode = customer.PostalCode,
            CreatedAt = customer.CreatedAt
        };
    }

    // IQueryable to DTOs using LINQ query syntax
    public static IQueryable<CustomerDto> ToDto(this IQueryable<Customer> customers)
    {
        return from customer in customers
               select new CustomerDto
               {
                   Id = customer.Id,
                   CustomerName = customer.CustomerName,
                   Email = customer.Email,
                   PhoneNumber = customer.PhoneNumber,
                   Address = customer.Address,
                   City = customer.City,
                   Country = customer.Country,
                   PostalCode = customer.PostalCode,
                   CreatedAt = customer.CreatedAt
               };
    }

    // CreateDto to Entity
    public static Customer ToEntity(this CreateCustomerDto dto)
    {
        return new Customer
        {
            CustomerName = dto.CustomerName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            City = dto.City,
            Country = dto.Country,
            PostalCode = dto.PostalCode
        };
    }

    // UpdateDto to Entity
    public static void UpdateEntity(this UpdateCustomerDto dto, Customer customer)
    {
        customer.CustomerName = dto.CustomerName;
        customer.Email = dto.Email;
        customer.PhoneNumber = dto.PhoneNumber;
        customer.Address = dto.Address;
        customer.City = dto.City;
        customer.Country = dto.Country;
        customer.PostalCode = dto.PostalCode;
    }
}