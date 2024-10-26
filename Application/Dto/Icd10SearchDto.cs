namespace Application.Dto;

public record Icd10SearchDto(
    List<Icd10RecordDto> records,
    PageInfoDto pagination
    );