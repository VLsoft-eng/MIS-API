using Application.Abstractions.Service;
using Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;

[ApiController]
[Route("api/dictionary/")]
public class DictionaryController
{
    private readonly ISpecialityService _specialityService;

    public DictionaryController (ISpecialityService specialityService)
    {
        _specialityService = specialityService;
    }

    [HttpGet("speciality")]
    public async Task<SpecialitiesPagedListDto> GetByNameAndParams([FromQuery] int page = 1,
        [FromQuery] int size = 5,
        [FromQuery] string name = "")
    {
        return await _specialityService.GetByNameAndParams(name, page, size);
    }
}