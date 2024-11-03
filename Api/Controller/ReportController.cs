using Application.Abstractions.Service;
using Application.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;

[ApiController]
[Route("api/report")]
public class ReportController(IReportService reportService) : ControllerBase
{
    [Authorize]
    [HttpGet("icdrootsreport")]
    public async Task<ActionResult<IcdRootsReportDto>> GetIcdRootsReport(
        [FromQuery] DateTime start,
        [FromQuery] DateTime end,
        [FromQuery] List<Guid>? icdRoots)
    {
        if (icdRoots == null || !icdRoots.Any())
        {
            icdRoots = new List<Guid>();
        }
        
        return await reportService.GetReport(start, end, icdRoots);
    }
}