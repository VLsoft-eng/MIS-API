using Api.Extensions;
using Application.Abstractions.Service;
using Application.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;

[ApiController]
[Route("api/patient/")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [Authorize]
    [HttpPost("create")]
    public async Task CreatePatient([FromBody] PatientCreateRequest request)
    {
        await _patientService.Create(request);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<PatientDto>> GetPatientById(Guid id)
    {
        return await _patientService.GetPatientById(id);
    }

    [Authorize]
    [HttpPost("{id}/inspections")]
    public async Task<ActionResult<Guid>> CreateInspectionForPatient(Guid id,
        [FromBody] InspectionCreateRequest request)
    {
        Guid doctorId = Guid.Parse(HttpContext.GetUserId());
        return await _patientService.CreatePatientsInspection(id, doctorId, request);
    }

    [Authorize]
    [HttpGet("{id}/inspections/search")]
    public async Task<List<InspectionShortDto>> GetPatientInspectionWithoutChilds(Guid id, [FromQuery] string request)
    {
        return await _patientService.SearchPatientInspectionsByParams(id, request);
    }
}