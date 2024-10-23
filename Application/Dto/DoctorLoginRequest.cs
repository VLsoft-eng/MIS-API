namespace Application.Dto;

public record DoctorLoginRequest(
    string email,
    string password);