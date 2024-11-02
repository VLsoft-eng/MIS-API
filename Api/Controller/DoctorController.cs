using Application.Abstractions.Service;
using Application.Dto;
using Microsoft.AspNetCore.Mvc;
using Api.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controller;

[ApiController]
[Route("api/doctor/")]
public class DoctorController : ControllerBase
{
    private readonly IDoctorService _doctorService;

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

    [Authorize]
    [HttpPost("logout")]
    public async Task Logout()
    {
        Guid tokenId = Guid.Parse(HttpContext.GetTokenId());
        string tokenValue = HttpContext.GetJwtToken();
        DateTime expiresAt = HttpContext.GetTokenExpiration().Value;
        
        await _doctorService.Logout(tokenId, tokenValue, expiresAt);
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> Login([FromBody] DoctorLoginRequest request)
    {
        return await _doctorService.Login(request);
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<DoctorDto>> GetProfile()
    {
        Guid id = Guid.Parse(HttpContext.GetUserId());
        var doctorDto = await _doctorService.GetDoctorInfo(id);
        return Ok(doctorDto);
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task EditProfile([FromBody] DoctorEditRequest request)
    {
        Guid id = Guid.Parse(HttpContext.GetUserId());
        
        await _doctorService.EditUserProfile(id, request);
    }
    

}