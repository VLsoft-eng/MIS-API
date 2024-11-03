using Application.Abstractions.Service;
using Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;

[ApiController]
[Route("api/dictionary/")]
public class DictionaryController(ISpecialityService specialityService, IIcdService icdService)
    : ControllerBase
{
    [HttpGet("speciality")]
    public async Task<ActionResult<SpecialitiesPagedListDto>> GetSpecialitiesByNameAndParams(
        [FromQuery] string? name,
        [FromQuery] int page = 1,
        [FromQuery] int size = 5)
    {
        var pagedList = await specialityService.GetByNameAndParams(name, page, size);
        return pagedList;
    }

    [HttpGet("icd10/root")]
    public async Task<ActionResult<List<Icd10RecordDto>>> GetRootElements()
    {
        return await icdService.GetRootElements();
    }

    [HttpGet("icd10")]
    public async Task<ActionResult<Icd10SearchDto>> GetIcdByNameAndParams(
        [FromQuery] string? name, 
        [FromQuery] int page = 1,
        [FromQuery] int size = 5)
    {
        var pagedList = await icdService.GetByNameAndParams(name, page, size);
        return Ok(pagedList);
    }
}