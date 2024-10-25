namespace Application.Dto;

public record SpecialitiesPagedListDto(
    List<SpecialityDto> specialities,
    PageInfoDto pagination);