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
}