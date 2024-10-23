using Application.Dto;

namespace Application.Abstractions.Service;

public interface IDoctorService
{
    Task<TokenDto> Register(DoctorRegistrationRequest request);
    Task<TokenDto> Login(DoctorLoginRequest request);
    Task<DoctorDto> getDoctorInfo(Guid id);
    Task editUserProfile(Guid id, DoctorEditRequest request);
}