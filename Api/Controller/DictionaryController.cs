using Application.Abstractions.Service;
using Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;

[ApiController]
[Route("api/dictionary/")]
public class DictionaryController : ControllerBase
{
    private readonly ISpecialityService _specialityService;
    private readonly IIcdService _icdService;

    public DictionaryController (ISpecialityService specialityService, IIcdService icdService)
    {
        _specialityService = specialityService;
        _icdService = icdService;
    }

    [HttpGet("speciality")]
    public async Task<ActionResult<SpecialitiesPagedListDto>> GetSpecialitiesByNameAndParams(
        [FromQuery] string? name,
        [FromQuery] int page = 1,
        [FromQuery] int size = 5)
    {
        var pagedList = await _specialityService.GetByNameAndParams(name, page, size);
        return pagedList;
    }

    [HttpGet("icd10/root")]
    public async Task<ActionResult<List<Icd10RecordDto>>> GetRootElements()
    {
        return await _icdService.GetRootElements();
    }

    [HttpGet("icd10")]
    public async Task<ActionResult<Icd10SearchDto>> GetIcdByNameAndParams(
        [FromQuery] string? name, 
        [FromQuery] int page = 1,
        [FromQuery] int size = 5)
    {
        var pagedList = await _icdService.GetByNameAndParams(name, page, size);
        return Ok(pagedList);
    }
}