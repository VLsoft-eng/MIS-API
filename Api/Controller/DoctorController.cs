using Application.Abstractions.Service;
using Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;

[ApiController]
[Route("api/doctor/")]
public class DoctorController : ControllerBase
{
    private IDoctorService _doctorService;

    public DoctorController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<TokenDto>> Register([FromBody] DoctorRegistrationRequest request)
    {
        var tokenDto = await _doctorService.Register (request);

        return Ok(tokenDto);
    }
    

}