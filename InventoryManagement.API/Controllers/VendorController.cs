using InventoryManagement.Application.DTOs.Common;
using InventoryManagement.Application.DTOs.Vendors;
using InventoryManagement.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class VendorController : ControllerBase
{
    private readonly IVendorService _vendorService;

    public VendorController(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVendors([FromQuery] PaginationParams pagination)
    {
        var result = await _vendorService.GetAllVendorsAsync(pagination);
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAllVendorsList()
    {
        var result = await _vendorService.GetAllVendorsAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVendorById(int id)
    {
        var result = await _vendorService.GetVendorByIdAsync(id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    public async Task<IActionResult> CreateVendor([FromBody] CreateVendorDto dto)
    {
        var result = await _vendorService.CreateVendorAsync(dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetVendorById), new { id = result.Data!.Id }, result);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVendor(int id, [FromBody] UpdateVendorDto dto)
    {
        var result = await _vendorService.UpdateVendorAsync(id, dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVendor(int id)
    {
        var result = await _vendorService.DeleteVendorAsync(id);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}