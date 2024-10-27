namespace Application.Dto;

public record InspectionPagedListDto(
    List<InspectionFullDto> inspections,
    PageInfoDto pagination);