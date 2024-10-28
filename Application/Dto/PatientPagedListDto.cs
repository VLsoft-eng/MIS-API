namespace Application.Dto;

public record PatientPagedListDto(
    List<PatientDto> patients,
    PageInfoDto pagination);