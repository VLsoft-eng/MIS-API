namespace Application.Dto;

public record ErrorResponse(
    int statusCode,
    string message);