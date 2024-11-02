using Api.Extensions;
using Application.Abstractions.Service;
using Application.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;


[ApiController]
[Route("api/inspection")]
public class InspectionController : ControllerBase
{
    private readonly IInspectionService _inspectionService;
    public InspectionController(IInspectionService inspectionService)
    {
        _inspectionService = inspectionService;
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<InspectionDto>> GetById(Guid id)
    {
        return await _inspectionService.GetInspectionById(id);
    }
    
    [Authorize]
    [HttpPut("{id}")]
    public async Task EditInspection(Guid id, [FromBody] InspectionEditRequest request)
    {
        var  doctorId = Guid.Parse(HttpContext.GetUserId());
        await _inspectionService.EditInspection(id, request, doctorId);
    }

    [Authorize]
    [HttpGet("{id}/chain")]
    public async Task<ActionResult<List<InspectionFullDto>>> GetInspectionChain(Guid id)
    {
        return await _inspectionService.GetChainByRoot(id);
    }
}