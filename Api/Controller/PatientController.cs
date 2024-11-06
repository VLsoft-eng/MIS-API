using Api.Extensions;
using Application.Abstractions.Service;
using Application.BusinessLogic.Enums;
using Application.Dto;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;

[ApiController]
[Route("api/patient")]
public class PatientController(IPatientService patientService) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<Guid> CreatePatient([FromBody] PatientCreateRequest request)
    {
        return await patientService.Create(request);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<PatientDto>> GetPatientById(Guid id)
    {
        return await patientService.GetPatientById(id);
    }

    [Authorize]
    [HttpPost("{id}/inspections")]
    public async Task<ActionResult<Guid>> CreateInspectionForPatient(Guid id,
        [FromBody] InspectionCreateRequest request)
    {
        Guid doctorId = Guid.Parse(HttpContext.GetUserId());
        return await patientService.CreatePatientsInspection(id, doctorId, request);
    }

    [Authorize]
    [HttpGet("{id}/inspections/search")]
    public async Task<ActionResult<List<InspectionShortDto>>> GetPatientInspectionWithoutChilds(Guid id, [FromQuery] string? request)
    {
        return await patientService.SearchPatientInspectionsByParams(id, request);
    }

    [Authorize]
    [HttpGet("{id}/inspections")]
    public async Task<ActionResult<InspectionPagedListDto>> GetPatientInspectionsByParams(
        Guid id,
        [FromQuery] bool grouped, 
        [FromQuery] List<Guid> icdRoots,
        [FromQuery] int page = 1,
        [FromQuery] int size = 5)
    {
        return await patientService.GetPatientInspectionsByParams(id, grouped, icdRoots, page, size);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PatientPagedListDto>> GetPatientList(
        [FromQuery] string? name,
        [FromQuery] List<Conclusion>? conclusions,
        [FromQuery] SortingType? sorting,
        [FromQuery] bool scheduledVisits = false,
        [FromQuery] bool onlyMine = false,
        [FromQuery] int page = 1,
        [FromQuery] int size = 5)
    {
        Guid doctorId = Guid.Parse(HttpContext.GetUserId());
        
        return await patientService.GetPatientsByParams(name, conclusions, sorting, scheduledVisits, onlyMine, page,
            size, doctorId);
    }
    
}