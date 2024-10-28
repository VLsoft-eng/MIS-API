using Application.Abstractions.Service;
using Application.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;

[ApiController]
[Route("/api/consultation")]
public class ConsultationController : ControllerBase
{
    private readonly IConsultationService _consultationService;

    public ConsultationController(IConsultationService consultationService)
    {
        _consultationService = consultationService;
    }

    [Authorize]
    [HttpPut("/comment/{id}")]
    public async Task EditComment(Guid id, [FromBody] CommentEditRequest request)
    {
        await _consultationService.UpdateComment(id, request);
    }
    
}