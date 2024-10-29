using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.Dto;
using Application.Exceptions;

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
        IInspectionMapper inspectionMapper)
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
}