using Api.Extensions;
using Application.Abstractions.Service;
using Application.Dto;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;

[ApiController]
[Route("api/consultation")]
public class ConsultationController(
    IConsultationService consultationService,
    ICommentService commentService,
    IInspectionService inspectionService)
    : ControllerBase
{
    [Authorize]
    [HttpPut("comment/{id}")]
    public async Task EditComment(Guid id, [FromBody] CommentEditRequest request)
    {
        Guid doctorId = Guid.Parse(HttpContext.GetUserId());
        await commentService.UpdateComment(id, doctorId,request);
    }

    [Authorize]
    [HttpPost("{id}/comment")]
    public async Task<ActionResult<Guid>> CreateComment(Guid id, [FromBody] ConsultationCommentCreateRequest request)
    {
        Guid doctorId = Guid.Parse(HttpContext.GetUserId());
        return await commentService.CreateComment(id, doctorId, request);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ConsultationDto>> GetConsultationById(Guid id)
    {
        return await consultationService.GetConsultation(id);
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
        return await inspectionService.GetInspectionsWithDoctorSpeciality(doctorId, grouped, icdRoots, page, size);
    }
    
   
}