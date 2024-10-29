using Application.Abstractions.Service;
using Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;


[ApiController]
[Route("api/inspection")]
public class InspectionController
{
    private readonly IInspectionService _inspectionService;

    public InspectionController(IInspectionService inspectionService)
    {
        _inspectionService = inspectionService;
    }

    [HttpGet("{id}")]
    public async Task<InspectionDto> GetById(Guid id)
    {
        return await _inspectionService.GetInspectionById(id);
    }
}