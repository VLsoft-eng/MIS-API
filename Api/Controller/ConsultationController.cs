using Api.Extensions;
using Application.Abstractions.Service;
using Application.Dto;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;

[ApiController]
[Route("api/consultation")]
public class ConsultationController : ControllerBase
{
    private readonly IConsultationService _consultationService;

    public ConsultationController(IConsultationService consultationService)
    {
        _consultationService = consultationService;
    }

    [Authorize]
    [HttpPut("comment/{id}")]
    public async Task EditComment(Guid id, [FromBody] CommentEditRequest request)
    {
        await _consultationService.UpdateComment(id, request);
    }

    [Authorize]
    [HttpPost("{id}/comment")]
    public async Task<Guid> CreateComment(Guid id, [FromBody] ConsultationCommentCreateRequest request)
    {
        Guid doctorId = Guid.Parse(HttpContext.GetUserId());
        return await _consultationService.CreateComment(id, doctorId, request);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ConsultationDto> GetConsultationById(Guid id)
    {
        return await _consultationService.GetConsultation(id);
    }
    
   
}