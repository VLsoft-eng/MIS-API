using Application.Abstractions.Auth;
using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.Dto;
using Application.Exceptions;
using Domain;
using FluentValidation;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace Application.BusinessLogic.Service;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly IValidator<DoctorRegistrationRequest> _registrationValidator;
    private readonly IValidator<DoctorLoginRequest> _loginValidator;
    private readonly IValidator<DoctorEditRequest> _doctorEditValidator;
    private readonly IDoctorMapper _doctorMapper;
    private readonly ITokenRepository _tokenRepository;
    private readonly ITokenMapper _tokenMapper;

    public DoctorService(
        IDoctorRepository doctorRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        IValidator<DoctorRegistrationRequest> registrationValidator,
        IValidator<DoctorLoginRequest> loginValidator,
        IValidator<DoctorEditRequest> doctorEditValidator,
        IDoctorMapper doctorMapper,
        ITokenRepository tokenRepository,
        ITokenMapper tokenMapper
    )
    {
        _doctorRepository = doctorRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _registrationValidator = registrationValidator;
        _loginValidator = loginValidator;
        _doctorEditValidator = doctorEditValidator;
        _doctorMapper = doctorMapper;
        _tokenRepository = tokenRepository;
        _tokenMapper = tokenMapper;
    }

    public async Task<TokenDto> Register(DoctorRegistrationRequest request)
    {
        var validation = await _registrationValidator.ValidateAsync(request);

        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors[0].ErrorMessage);
        }

        var existingDoctor = await _doctorRepository.GetByEmail(request.email);

        if (existingDoctor != null)
        {
            throw new EmailAlreadyUsedException();
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
            throw new ValidationException(validation.Errors[0].ErrorMessage);
        }

        var doctor = await _doctorRepository.GetByEmail(request.email);
        if (doctor == null)
        {
            throw new InvalidAuthCredentialsException();
        }

        var isPasswordEqual = _passwordHasher.Verify(request.password, doctor.hashedPassword);
        if (!isPasswordEqual)
        {
            throw new InvalidAuthCredentialsException();
        }

        var token = _jwtProvider.GenerateToken(doctor);
        var tokenDto = new TokenDto(token);

        return tokenDto;
    }

    public async Task<DoctorDto> GetDoctorInfo(Guid id)
    {
        var doctor = await _doctorRepository.GetById(id);
        if (doctor == null)
        {
            throw new DoctorNotFoundException();
        }

        var dto = _doctorMapper.ToDto(doctor);
        return dto;
    }

    public async Task EditUserProfile(Guid id, DoctorEditRequest request)
    {
        var doctor = await _doctorRepository.GetById(id);
        if (doctor == null)
        {
            throw new DoctorNotFoundException();
        }

        var validation = await _doctorEditValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors[0].ErrorMessage);
        }
        
        _doctorMapper.UpdateDoctorEntity(doctor, request);
        await _doctorRepository.Update(doctor);
    }

    public async Task Logout(Guid tokenId, string tokenValue)
    {
        var token = await _tokenRepository.GetById(tokenId);

        if (token != null)
        {
            throw new AlreadyLogoutException();
        }

        Token bannedToken = _tokenMapper.ToEntity (tokenId, tokenValue);
        await _tokenRepository.Add(bannedToken);
    }
}