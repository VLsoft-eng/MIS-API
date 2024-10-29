using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.BusinessLogic.Validation;
using Application.Dto;
using Application.Exceptions;
using Domain;
using FluentValidation;

namespace Application.BusinessLogic.Service;

public class InspectionService : IInspectionService
{
    private readonly IInspectionRepository _inspectionRepository;
    private readonly IDiagnosisRepository _diagnosisRepository;
    private readonly IConsultationRepository _consultationRepository;
    private readonly IDoctorMapper _doctorMapper;
    private readonly IPatientMapper _patientMapper;
    private readonly IDiagnosisMapper _diagnosisMapper;
    private readonly IConsultationMapper _consultationMapper;
    private readonly ISpecialityMapper _specialityMapper;
    private readonly ICommentRepository _commentRepository;
    private readonly ICommentMapper _commentMapper;
    private readonly IInspectionMapper _inspectionMapper;
    private readonly IValidator<InspectionEditRequest> _inspectionEditValidator;
    private readonly IValidator<DiagnosisCreateRequest> _diagnosisCreateValidator;
    private readonly IIcdRepository _icdRepository;
    
    public InspectionService(
        IInspectionRepository inspectionRepository,
        IDiagnosisRepository diagnosisRepository,
        IConsultationRepository consultationRepository,
        IDoctorMapper doctorMapper,
        IPatientMapper patientMapper,
        IDiagnosisMapper diagnosisMapper,
        IConsultationMapper consultationMapper,
        ISpecialityMapper specialityMapper,
        ICommentRepository commentRepository,
        ICommentMapper commentMapper,
        IInspectionMapper inspectionMapper,
        IValidator<InspectionEditRequest> inspectionEditValidator,
        IValidator<DiagnosisCreateRequest> diagnosisCreateValidator,
        IIcdRepository icdRepository)
    {
        _inspectionRepository = inspectionRepository;
        _diagnosisRepository = diagnosisRepository;
        _consultationRepository = consultationRepository;
        _doctorMapper = doctorMapper;
        _patientMapper = patientMapper;
        _diagnosisMapper = diagnosisMapper;
        _consultationMapper = consultationMapper;
        _specialityMapper = specialityMapper;
        _commentRepository = commentRepository;
        _commentMapper = commentMapper;
        _inspectionMapper = inspectionMapper;
        _inspectionEditValidator = inspectionEditValidator;
        _diagnosisCreateValidator = diagnosisCreateValidator;
        _icdRepository = icdRepository;
    }

    public async Task<InspectionDto> GetInspectionById(Guid id)
    {
        var inspection = await _inspectionRepository.GetById(id);
        if (inspection == null)
        {
            throw new InspectionNotFoundException();
        }

        var diagnoses = await _diagnosisRepository.GetDiagnosesByInspectionId(id);
        var consultations = await _consultationRepository.GetByInspectionId(id);
        var baseInspection = await _inspectionRepository.GetBaseInspection(id);
        var patient = inspection.patient;
        var doctor = inspection.doctor;

        var doctorDto = _doctorMapper.ToDto(doctor);
        var patientDto = _patientMapper.ToDto(patient);
        var diagnosisDtos = diagnoses.Select(d => _diagnosisMapper.ToDto(d)).ToList();
        
        var consultationDtos = new List<InspectionConsultationDto>();
        foreach (var consultation in consultations)
        {
            var consultationComments = await _commentRepository.GetCommentsByConsultationId(consultation.id);
            var rootComment = consultationComments.Where(c => c.parent == null).ToList()[0];
            var authorDoctorDto = _doctorMapper.ToDto(rootComment.author);
            var consultationSpecialityDto = _specialityMapper.ToDto(consultation.speciality);
            var rootCommentDto = _commentMapper.ToInspectionCommentDto(rootComment, authorDoctorDto);
            
            var consultationDto = _consultationMapper.ToInspectionConsultationDto(
                consultation,
                consultationSpecialityDto,
                rootCommentDto,
                consultation.inspection,
                consultationComments.Count);
            consultationDtos.Add(consultationDto);
        }
        
        return _inspectionMapper.ToDto(inspection, baseInspection == null ? null : baseInspection.id, patientDto, doctorDto, diagnosisDtos,
            consultationDtos);
    }

    public async Task EditInspection(Guid inspectionId, InspectionEditRequest request, Guid doctorId)
    {
        var inspection = await _inspectionRepository.GetById(inspectionId);
        if (inspection == null)
        {
            throw new InspectionNotFoundException();
        }

        if (inspection.doctor.id != doctorId)
        {
            throw new DoesntHaveRightsException();
        }

        var validation = await _inspectionEditValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors[0].ErrorMessage);
        }
        
        var newDiagnoses = request.diagnoses;
        var newDiagnosesEntities = new List<Diagnosis>();
        foreach (var diagnosis in newDiagnoses)
        {
            var diagnosisValidation = await _diagnosisCreateValidator.ValidateAsync(diagnosis);
            if (!diagnosisValidation.IsValid)
            {
                throw new ValidationException(diagnosisValidation.Errors[0].ErrorMessage);
            }

            var icd = await _icdRepository.GetById(diagnosis.icdDiagnosisId);
            if (icd == null)
            {
                throw new IcdNotFoundException();
            }

            var diagnosisEntity = _diagnosisMapper.ToEntity(diagnosis, icd, inspection);
            newDiagnosesEntities.Add(diagnosisEntity);
        }
        
        _inspectionMapper.UpdateInspectionEntity(inspection, request);
        await _inspectionRepository.Update(inspection);


        await _diagnosisRepository.DeleteAllInspectionDiagnoses(inspectionId);
        await _diagnosisRepository.CreateRange(newDiagnosesEntities);
    }
}