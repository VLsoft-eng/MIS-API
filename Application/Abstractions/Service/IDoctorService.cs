using Application.Dto;

namespace Application.Abstractions.Service;

public interface IDoctorService
{
    Task<TokenDto> Register(DoctorRegistrationRequest request);
    Task<TokenDto> Login(DoctorLoginRequest request);
    Task<DoctorDto> GetDoctorInfo(Guid id);
    Task EditUserProfile(Guid id, DoctorEditRequest request);
    Task Logout(Guid tokenId, string tokenValue, DateTime expiredTime);
}