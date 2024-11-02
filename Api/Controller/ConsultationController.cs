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
    private readonly ICommentService _commentService;
    private readonly IInspectionService _inspectionService;

    public ConsultationController(
        IConsultationService consultationService,
        ICommentService commentService,
        IInspectionService inspectionService)
    {
        _consultationService = consultationService;
        _commentService = commentService;
        _inspectionService = inspectionService;
    }

    [Authorize]
    [HttpPut("comment/{id}")]
    public async Task EditComment(Guid id, [FromBody] CommentEditRequest request)
    {
        await _commentService.UpdateComment(id, request);
    }

    [Authorize]
    [HttpPost("{id}/comment")]
    public async Task<ActionResult<Guid>> CreateComment(Guid id, [FromBody] ConsultationCommentCreateRequest request)
    {
        Guid doctorId = Guid.Parse(HttpContext.GetUserId());
        return await _commentService.CreateComment(id, doctorId, request);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ConsultationDto>> GetConsultationById(Guid id)
    {
        return await _consultationService.GetConsultation(id);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<InspectionPagedListDto>> GetDoctorsSpecialityInspections(
        [FromQuery] bool grouped,
        [FromQuery] List<Guid> icdRoots,
        [FromQuery] int page = 1,
        [FromQuery] int size = 5)
    {
        Guid doctorId = Guid.Parse(HttpContext.GetUserId());
        return await _inspectionService.GetInspectionsWithDoctorSpeciality(doctorId, grouped, icdRoots, page, size);
    }
    
   
}