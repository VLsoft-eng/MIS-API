namespace Application.Dto;

public record PageInfoDto(
    int size,
    int count,
    int current);