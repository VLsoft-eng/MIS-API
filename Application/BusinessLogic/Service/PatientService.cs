using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.Dto;
using FluentValidation;

namespace Application.BusinessLogic.Service;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IPatientMapper _patientMapper;
    private readonly IValidator<PatientCreateRequest> _patientCreateValidator;

    public PatientService(
        IPatientRepository patientRepository,
        IPatientMapper patientMapper,
        IValidator<PatientCreateRequest> patientCreateValidator)
    {
        _patientRepository = patientRepository;
        _patientMapper = patientMapper;
        _patientCreateValidator = patientCreateValidator;
    }

    public async Task Create(PatientCreateRequest request)
    {
        var validation = await _patientCreateValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors[0].ToString());
        }

        var patient = _patientMapper.ToEntity(request);
        await _patientRepository.Create(patient);
    }

    public async Task<PatientDto> GetPatientById(Guid id)
    {
        var patient = await _patientRepository.GetById(id);
        if (patient == null)
        {
            throw new Exception();
        }

        var patientDto = _patientMapper.ToDto(patient);
        return patientDto;
    }
}