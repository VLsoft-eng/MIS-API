using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.Dto;
using Application.Exceptions;
using Domain;
using FluentValidation;

namespace Application.BusinessLogic.Service;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IPatientMapper _patientMapper;
    private readonly IValidator<PatientCreateRequest> _patientCreateValidator;
    private readonly IValidator<InspectionCreateRequest> _inspectionCreateValidator;
    private readonly IValidator<ConsultationCreateRequest> _consultationCreateValidator;
    private readonly IValidator<InspectionCommentCreateRequest> _inspectionCreateCommentValidator;
    private readonly IValidator<DiagnosisCreateRequest> _diagnosisCreateValidator;
    private readonly IIcdRepository _icdRepository;
    private readonly ISpecialityRepository _specialityRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IInspectionRepository _inspectionRepository;
    private readonly IConsultationRepository _consultationRepository;
    private readonly IDiagnosisRepository _diagnosisRepository;
    private readonly IInspectionMapper _inspectionMapper;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDiagnosisMapper _diagnosisMapper;
    private readonly ICommentMapper _commentMapper;
    private readonly IConsultationMapper _consultationMapper;

    public PatientService(
        IPatientRepository patientRepository,
        IPatientMapper patientMapper,
        IValidator<PatientCreateRequest> patientCreateValidator,
        IValidator<InspectionCreateRequest> inspectionCreateValidator,
        IValidator<ConsultationCreateRequest> consultationCreateValidator,
        IValidator<InspectionCommentCreateRequest> inspectionCreateCommentValidator,
        IValidator<DiagnosisCreateRequest> diagnosisCreateValidator,
        IIcdRepository icdRepository,
        ISpecialityRepository specialityRepository,
        ICommentRepository commentRepository,
        IInspectionRepository inspectionRepository,
        IConsultationRepository consultationRepository,
        IDiagnosisRepository diagnosisRepository,
        IInspectionMapper inspectionMapper,
        IDoctorRepository doctorRepository,
        IDiagnosisMapper diagnosisMapper,
        ICommentMapper commentMapper,
        IConsultationMapper consultationMapper)
    {
        _patientRepository = patientRepository;
        _patientMapper = patientMapper;
        _patientCreateValidator = patientCreateValidator;
        _inspectionCreateValidator = inspectionCreateValidator;
        _consultationCreateValidator = consultationCreateValidator;
        _inspectionCreateCommentValidator = inspectionCreateCommentValidator;
        _diagnosisCreateValidator = diagnosisCreateValidator;
        _icdRepository = icdRepository;
        _specialityRepository = specialityRepository;
        _commentRepository = commentRepository;
        _inspectionRepository = inspectionRepository;
        _consultationRepository = consultationRepository;
        _diagnosisRepository = diagnosisRepository;
        _inspectionMapper = inspectionMapper;
        _doctorRepository = doctorRepository;
        _diagnosisMapper = diagnosisMapper;
        _commentMapper = commentMapper;
        _consultationMapper = consultationMapper;
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
            throw new PatientNotFoundException();
        }

        var patientDto = _patientMapper.ToDto(patient);
        return patientDto;
    }

    public async Task<Guid> CreatePatientsInspection(Guid patientId, Guid doctorId, InspectionCreateRequest request)
    {
        var patient = await _patientRepository.GetById(patientId);
        if (patient == null)
        {
            throw new PatientNotFoundException();
        }

        var inspectionValidation = await _inspectionCreateValidator.ValidateAsync(request);
        if (!inspectionValidation.IsValid)
        {
            throw new ValidationException(inspectionValidation.Errors[0].ToString());
        }

        Inspection previousInspection = null;

        if (request.previousInspectionId != null)
        {
            var previousInspectionId = request.previousInspectionId ?? Guid.NewGuid();
            previousInspection = await _inspectionRepository.GetById(previousInspectionId);
            if (previousInspection == null)
            {
                throw new InspectionNotFoundException();
            }
        }

        if (request.consultations != null)
        {
            foreach (var consultation in request.consultations)
            {
                var consultationValidation = await _consultationCreateValidator.ValidateAsync(consultation);
                if (!consultationValidation.IsValid)
                {
                    throw new ValidationException(consultationValidation.Errors[0].ToString());
                }

                var commentValidation = await _inspectionCreateCommentValidator.ValidateAsync(consultation.comment);
                if (!commentValidation.IsValid)
                {
                    throw new ValidationException(commentValidation.Errors[0].ToString());
                }

                var speciality = await _specialityRepository.GetById(consultation.specialityId);
                if (speciality == null)
                {
                    throw new SpecialityNotFoundException();
                }
            }
        }

        foreach (var diagnosis in request.diagnoses)
        {
            var diagnosisValidation = await _diagnosisCreateValidator.ValidateAsync(diagnosis);
            if (!diagnosisValidation.IsValid)
            {
                throw new ValidationException(diagnosisValidation.Errors[0].ToString());
            }

            var icd = await _icdRepository.GetById(diagnosis.icdDiagnosisId);

            if (icd == null)
            {
                throw new IcdNotFoundException();
            }
        }

        var doctor = await _doctorRepository.GetById(doctorId);

        Inspection inspection = _inspectionMapper.ToEntity(request, doctor, patient, previousInspection);

        await _inspectionRepository.Create(inspection);

        foreach (var diagnosis in request.diagnoses)
        {
            Icd icd = await _icdRepository.GetById(diagnosis.icdDiagnosisId);
            var diagnosisEntity = _diagnosisMapper.ToEntity(diagnosis, icd, inspection);
            await _diagnosisRepository.Create(diagnosisEntity);
        }

        if (request.consultations != null)
        {
            foreach (var consultationRequest in request.consultations)
            {
                Speciality speciality = await _specialityRepository.GetById(consultationRequest.specialityId);
                Consultation consultation = _consultationMapper.ToEntity(speciality, inspection);
                Comment comment = _commentMapper.ToEntity(consultationRequest.comment, doctor, consultation, null);

                await _consultationRepository.Create(consultation);
                await _commentRepository.Create(comment);
            }
        }

        return inspection.id;
    }
}