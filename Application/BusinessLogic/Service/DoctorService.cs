using Application.Abstractions.Auth;
using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.Dto;
using Application.Exceptions;
using Domain;
using FluentValidation;

namespace Application.BusinessLogic.Service;

public class DoctorService(
    IDoctorRepository doctorRepository,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider,
    IValidator<DoctorRegistrationRequest> registrationValidator,
    IValidator<DoctorLoginRequest> loginValidator,
    IValidator<DoctorEditRequest> doctorEditValidator,
    IDoctorMapper doctorMapper,
    ITokenRepository tokenRepository,
    ITokenMapper tokenMapper)
    : IDoctorService
{
    public async Task<TokenDto> Register(DoctorRegistrationRequest request)
    {
        var validation = await registrationValidator.ValidateAsync(request);

        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors[0].ErrorMessage);
        }

        var existingDoctor = await doctorRepository.GetByEmail(request.email);

        if (existingDoctor != null)
        {
            throw new EmailAlreadyUsedException();
        }
        var doctor = await doctorMapper.ToEntity(request);
        doctor.hashedPassword = passwordHasher.Generate(doctor.hashedPassword);

        await doctorRepository.Create(doctor);

        var token = jwtProvider.GenerateToken(doctor);
        var tokenDto = new TokenDto(token);

        return tokenDto;
    }

    public async Task<TokenDto> Login(DoctorLoginRequest request)
    {
        var validation = await loginValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors[0].ErrorMessage);
        }

        var doctor = await doctorRepository.GetByEmail(request.email);
        if (doctor == null)
        {
            throw new InvalidAuthCredentialsException();
        }

        var isPasswordEqual = passwordHasher.Verify(request.password, doctor.hashedPassword);
        if (!isPasswordEqual)
        {
            throw new InvalidAuthCredentialsException();
        }

        var token = jwtProvider.GenerateToken(doctor);
        var tokenDto = new TokenDto(token);

        return tokenDto;
    }

    public async Task<DoctorDto> GetDoctorInfo(Guid id)
    {
        var doctor = await doctorRepository.GetById(id);
        if (doctor == null)
        {
            throw new DoctorNotFoundException();
        }

        var dto = doctorMapper.ToDto(doctor);
        return dto;
    }

    public async Task EditUserProfile(Guid id, DoctorEditRequest request)
    {
        var doctor = await doctorRepository.GetById(id);
        if (doctor == null)
        {
            throw new DoctorNotFoundException();
        }

        var validation = await doctorEditValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors[0].ErrorMessage);
        }

        var doctorWithEqualEmail = await doctorRepository.GetByEmail(request.email);
        if (doctorWithEqualEmail != null)
        {
            throw new EmailAlreadyUsedException();
        }
        
        doctorMapper.UpdateDoctorEntity(doctor, request);
        await doctorRepository.Update(doctor);
    }

    public async Task Logout(Guid tokenId, string tokenValue, DateTime expiresTime)
    {
        var token = await tokenRepository.GetById(tokenId);

        if (token != null)
        {
            throw new AlreadyLogoutException();
        }

        Token bannedToken = tokenMapper.ToEntity(tokenId, tokenValue, expiresTime);
        await tokenRepository.Add(bannedToken);
    }
}