using System.ComponentModel.DataAnnotations;
using Application.Abstractions.Auth;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.BusinessLogic.Mapper;
using Application.BusinessLogic.Validation;
using Application.Dto;
using Domain;

namespace Application.BusinessLogic.Service;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly DoctorRegistrationValidator _registrationValidator;
    private readonly DoctorLoginValidator _loginValidator;
    private readonly DoctorEditValidator _doctorEditValidator;
    private readonly DoctorMapper _doctorMapper;

    public DoctorService(
        IDoctorRepository doctorRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        DoctorRegistrationValidator registrationValidator,
        DoctorMapper doctorMapper,
        DoctorLoginValidator loginValidator,
        DoctorEditValidator doctorEditValidator
    )
    {
        _doctorRepository = doctorRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _registrationValidator = registrationValidator;
        _doctorMapper = doctorMapper;
        _loginValidator = loginValidator;
        _doctorEditValidator = doctorEditValidator;
    }

    public async Task<TokenDto> Register(DoctorRegistrationRequest request)
    {
        var validation = await _registrationValidator.ValidateAsync(request);

        if (!validation.IsValid)
        {
            throw new ValidationException();
        }

        var doctor = await _doctorMapper.ToEntity(request);
        doctor.hashedPassword = _passwordHasher.Generate(doctor.hashedPassword);

        await _doctorRepository.Create(doctor);

        var token = _jwtProvider.GenerateToken(doctor);
        var tokenDto = new TokenDto(token);

        return tokenDto;
    }

    public async Task<TokenDto> Login(DoctorLoginRequest request)
    {
        var validation = await _loginValidator.ValidateAsync(request);

        if (!validation.IsValid)
        {
            throw new ValidationException();
        }

        var doctor = await _doctorRepository.GetByEmail(request.email);
        if (doctor == null)
        {
            throw new Exception();
        }

        var isPasswordEqual = _passwordHasher.Verify(request.password, doctor.hashedPassword);
        if (!isPasswordEqual)
        {
            throw new Exception();
        }

        var token = _jwtProvider.GenerateToken(doctor);
        var tokenDto = new TokenDto(token);

        return tokenDto;
    }

    public async Task<DoctorDto> getDoctorInfo(Guid id)
    {
        var doctor = await _doctorRepository.GetById(id);
        if (doctor == null)
        {
            throw new Exception();
        }

        var dto = _doctorMapper.ToDto(doctor);
        return dto;
    }

    public async Task editUserProfile(Guid id, DoctorEditRequest request)
    {
        var doctor = await _doctorRepository.GetById(id);
        if (doctor == null)
        {
            throw new Exception();
        }

        var validation = await _doctorEditValidator.ValidateAsync(request);
        
        _doctorMapper.UpdateDoctorEntity(doctor, request);
        await _doctorRepository.Update(doctor);
    }
}