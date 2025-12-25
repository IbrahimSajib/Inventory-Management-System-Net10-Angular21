using InventoryManagement.Application.DTOs.Common;
using InventoryManagement.Application.DTOs.Quotations;
using InventoryManagement.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class QuotationController : ControllerBase
{
    private readonly IQuotationService _quotationService;

    public QuotationController(IQuotationService quotationService)
    {
        _quotationService = quotationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllQuotations([FromQuery] PaginationParams pagination)
    {
        var result = await _quotationService.GetAllQuotationsAsync(pagination);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetQuotationById(int id)
    {
        var result = await _quotationService.GetQuotationByIdAsync(id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    public async Task<IActionResult> CreateQuotation([FromBody] CreateQuotationDto dto)
    {
        var result = await _quotationService.CreateQuotationAsync(dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetQuotationById), new { id = result.Data!.Id }, result);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuotation(int id, [FromBody] UpdateQuotationDto dto)
    {
        var result = await _quotationService.UpdateQuotationAsync(id, dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuotation(int id)
    {
        var result = await _quotationService.DeleteQuotationAsync(id);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}