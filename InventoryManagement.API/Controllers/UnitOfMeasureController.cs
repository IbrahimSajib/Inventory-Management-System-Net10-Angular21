using InventoryManagement.Application.DTOs.Common;
using InventoryManagement.Application.DTOs.UnitOfMeasures;
using InventoryManagement.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UnitOfMeasureController : ControllerBase
{
    private readonly IUnitOfMeasureService _unitOfMeasureService;

    public UnitOfMeasureController(IUnitOfMeasureService unitOfMeasureService)
    {
        _unitOfMeasureService = unitOfMeasureService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUnits([FromQuery] PaginationParams pagination)
    {
        var result = await _unitOfMeasureService.GetAllUnitsAsync(pagination);
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAllUnitsList()
    {
        var result = await _unitOfMeasureService.GetAllUnitsAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUnitById(int id)
    {
        var result = await _unitOfMeasureService.GetUnitByIdAsync(id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    public async Task<IActionResult> CreateUnit([FromBody] CreateUnitOfMeasureDto dto)
    {
        var result = await _unitOfMeasureService.CreateUnitAsync(dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetUnitById), new { id = result.Data!.Id }, result);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUnit(int id, [FromBody] UpdateUnitOfMeasureDto dto)
    {
        var result = await _unitOfMeasureService.UpdateUnitAsync(id, dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUnit(int id)
    {
        var result = await _unitOfMeasureService.DeleteUnitAsync(id);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}